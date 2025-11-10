import { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { transacaoService, contaService } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import { Layout } from '../components/Layout';
import { ArrowLeft } from 'lucide-react';
import type { Conta } from '../types';

export const NovaTransacao = () => {
  const { cliente } = useAuth();
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const contaIdParam = searchParams.get('contaId');

  const [contas, setContas] = useState<Conta[]>([]);
  const [formData, setFormData] = useState({
    contaId: contaIdParam || '',
    tipo: 'Credito',
    valor: '',
    descricao: '',
    contaDestinoId: '',
    referenceId: '',
  });
  const [loading, setLoading] = useState(false);
  const [erro, setErro] = useState('');

  useEffect(() => {
    if (cliente) {
      loadContas();
    }
  }, [cliente]);

  const loadContas = async () => {
    if (!cliente) return;
    try {
      const data = await contaService.obterContas(cliente.id);
      setContas(data);
    } catch (error) {
      console.error('Erro ao carregar contas:', error);
    }
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>
  ) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
  };

  const generateReferenceId = () => {
    return `REF-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErro('');

    if (!formData.contaId) {
      setErro('Selecione uma conta válida.');
      return;
    }

    const valorSanitizado = formData.valor.replace(/\s/g, '').replace(',', '.');
    const valorNumero = Number(valorSanitizado);

    if (Number.isNaN(valorNumero) || valorNumero <= 0) {
      setErro('Informe um valor válido maior que zero.');
      return;
    }

    const referenceId = formData.referenceId || generateReferenceId();

    setLoading(true);

    try {
      await transacaoService.processarTransacao({
        contaId: formData.contaId,
        referenceId,
        tipo: formData.tipo,
        valor: valorNumero,
        descricao: formData.descricao || undefined,
        contaDestinoId: formData.tipo === 'Transferencia' ? formData.contaDestinoId : undefined,
      });
      navigate(`/contas/${formData.contaId}`);
    } catch (error: any) {
      setErro(error.response?.data?.message || 'Erro ao processar transação');
    } finally {
      setLoading(false);
    }

    if (!formData.referenceId) {
      setFormData((prev) => ({ ...prev, referenceId }));
    }
  };

  return (
    <Layout>
      <div className="max-w-2xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <button
          onClick={() => navigate('/transacoes')}
          className="inline-flex items-center text-indigo-600 hover:text-indigo-700 mb-6"
        >
          <ArrowLeft className="w-4 h-4 mr-2" />
          Voltar
        </button>

        <div className="bg-white rounded-lg shadow p-8">
          <h1 className="text-2xl font-bold text-gray-900 mb-6">Nova Transação</h1>

          {erro && (
            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-6">
              {erro}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-6">
            <div>
              <label htmlFor="contaId" className="block text-sm font-medium text-gray-700 mb-2">
                Conta
              </label>
              <select
                id="contaId"
                name="contaId"
                value={formData.contaId}
                onChange={handleChange}
                required
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
              >
                <option value="">Selecione uma conta</option>
                {contas.map((conta) => (
                  <option key={conta.id} value={conta.id}>
                    {conta.numero} - Saldo: R$ {conta.saldoTotal.toLocaleString('pt-BR')}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label htmlFor="tipo" className="block text-sm font-medium text-gray-700 mb-2">
                Tipo de Transação
              </label>
              <select
                id="tipo"
                name="tipo"
                value={formData.tipo}
                onChange={handleChange}
                required
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
              >
                <option value="Credito">Crédito</option>
                <option value="Debito">Débito</option>
                <option value="Reserva">Reserva</option>
                <option value="Captura">Captura</option>
                <option value="Transferencia">Transferência</option>
              </select>
            </div>

            {formData.tipo === 'Transferencia' && (
              <div>
                <label
                  htmlFor="contaDestinoId"
                  className="block text-sm font-medium text-gray-700 mb-2"
                >
                  Conta Destino
                </label>
                <select
                  id="contaDestinoId"
                  name="contaDestinoId"
                  value={formData.contaDestinoId}
                  onChange={handleChange}
                  required={formData.tipo === 'Transferencia'}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                >
                  <option value="">Selecione a conta destino</option>
                  {contas
                    .filter((c) => c.id !== formData.contaId)
                    .map((conta) => (
                      <option key={conta.id} value={conta.id}>
                        {conta.numero}
                      </option>
                    ))}
                </select>
              </div>
            )}

            <div>
              <label htmlFor="valor" className="block text-sm font-medium text-gray-700 mb-2">
                Valor (R$)
              </label>
              <input
                id="valor"
                name="valor"
                type="number"
                step="0.01"
                min="0.01"
                value={formData.valor}
                onChange={handleChange}
                required
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                placeholder="0.00"
              />
            </div>

            <div>
              <label htmlFor="descricao" className="block text-sm font-medium text-gray-700 mb-2">
                Descrição (opcional)
              </label>
              <textarea
                id="descricao"
                name="descricao"
                value={formData.descricao}
                onChange={handleChange}
                rows={3}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                placeholder="Descrição da transação"
              />
            </div>

            <div className="flex gap-4">
              <button
                type="button"
                onClick={() => navigate('/transacoes')}
                className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50"
              >
                Cancelar
              </button>
              <button
                type="submit"
                disabled={loading}
                className="flex-1 bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {loading ? 'Processando...' : 'Processar Transação'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </Layout>
  );
};

