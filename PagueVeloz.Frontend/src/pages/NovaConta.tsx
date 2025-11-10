import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { contaService } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import { Layout } from '../components/Layout';
import { ArrowLeft } from 'lucide-react';
import { useToast } from '../contexts/ToastContext';

export const NovaConta = () => {
  const { cliente } = useAuth();
  const navigate = useNavigate();
  const { showToast } = useToast();
  const [limiteCredito, setLimiteCredito] = useState('');
  const [loading, setLoading] = useState(false);
  const [erro, setErro] = useState('');

  const erroPadrao = 'Erro ao criar conta';

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!cliente) return;

    setErro('');
    setLoading(true);

    try {
      if (!limiteCredito.trim()) {
        throw new Error('Informe um limite de crédito válido.');
      }

      const valorSanitizado = limiteCredito
        .replace(/\./g, '')
        .replace(',', '.')
        .replace(/\s/g, '');

      const valorLimite = Number(valorSanitizado);

      if (Number.isNaN(valorLimite)) {
        throw new Error('Informe um limite de crédito válido.');
      }

      const conta = await contaService.criarConta({
        clienteId: cliente.id,
        limiteCredito: valorLimite,
      });

      showToast('Conta criada com sucesso!', 'success');
      navigate(`/contas/${conta.id}`);
    } catch (error: any) {
      const apiMessage =
        error?.response?.data?.message ??
        (typeof error?.response?.data === 'string' ? error.response.data : null) ??
        erroPadrao;

      setErro(apiMessage);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Layout>
      <div className="max-w-2xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <button
          onClick={() => navigate('/contas')}
          className="inline-flex items-center text-indigo-600 hover:text-indigo-700 mb-6"
        >
          <ArrowLeft className="w-4 h-4 mr-2" />
          Voltar
        </button>

        <div className="bg-white rounded-lg shadow p-8">
          <h1 className="text-2xl font-bold text-gray-900 mb-6">Nova Conta</h1>

          {erro && (
            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-6">
              {erro}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-6">
            <div>
              <label htmlFor="limiteCredito" className="block text-sm font-medium text-gray-700 mb-2">
                Limite de Crédito (R$)
              </label>
              <input
                id="limiteCredito"
                type="number"
                step="0.01"
                min="0"
                value={limiteCredito}
                onChange={(e) => setLimiteCredito(e.target.value)}
                required
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                placeholder="0.00"
              />
            </div>

            <div className="flex gap-4">
              <button
                type="button"
                onClick={() => navigate('/contas')}
                className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50"
              >
                Cancelar
              </button>
              <button
                type="submit"
                disabled={loading}
                className="flex-1 bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {loading ? 'Criando...' : 'Criar Conta'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </Layout>
  );
};

