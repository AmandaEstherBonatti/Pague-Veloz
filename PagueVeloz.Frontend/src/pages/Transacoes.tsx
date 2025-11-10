import { useEffect, useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { contaService, transacaoService } from '../services/api';
import { Layout } from '../components/Layout';
import { Link } from 'react-router-dom';
import { Plus, Filter } from 'lucide-react';
import type { Conta, Extrato } from '../types';
import { format } from 'date-fns';
import ptBR from 'date-fns/locale/pt-BR';

export const Transacoes = () => {
  const { cliente } = useAuth();
  const [contas, setContas] = useState<Conta[]>([]);
  const [contaSelecionada, setContaSelecionada] = useState<string>('');
  const [extrato, setExtrato] = useState<Extrato | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (cliente) {
      loadContas();
    }
  }, [cliente]);

  useEffect(() => {
    if (contaSelecionada) {
      loadExtrato();
    }
  }, [contaSelecionada]);

  const loadContas = async () => {
    if (!cliente) return;
    try {
      const data = await contaService.obterContas(cliente.id);
      setContas(data);
      if (data.length > 0) {
        setContaSelecionada(data[0].id);
      }
    } catch (error) {
      console.error('Erro ao carregar contas:', error);
    }
  };

  const loadExtrato = async () => {
    if (!contaSelecionada) return;
    setLoading(true);
    try {
      const data = await transacaoService.obterExtrato(contaSelecionada);
      setExtrato(data);
    } catch (error) {
      console.error('Erro ao carregar extrato:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Layout>
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="mb-8 flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Transações</h1>
            <p className="text-gray-600 mt-2">Histórico completo de transações</p>
          </div>
          <Link
            to="/transacoes/nova"
            className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 flex items-center gap-2"
          >
            <Plus className="w-5 h-5" />
            Nova Transação
          </Link>
        </div>

        <div className="bg-white rounded-lg shadow p-6 mb-6">
          <div className="flex items-center gap-4">
            <Filter className="w-5 h-5 text-gray-400" />
            <label htmlFor="conta" className="text-sm font-medium text-gray-700">
              Filtrar por conta:
            </label>
            <select
              id="conta"
              value={contaSelecionada}
              onChange={(e) => setContaSelecionada(e.target.value)}
              className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
            >
              <option value="">Todas as contas</option>
              {contas.map((conta) => (
                <option key={conta.id} value={conta.id}>
                  {conta.numero}
                </option>
              ))}
            </select>
          </div>
        </div>

        {loading ? (
          <div className="bg-white rounded-lg shadow p-8 text-center text-gray-500">
            Carregando...
          </div>
        ) : extrato ? (
          <div className="bg-white rounded-lg shadow overflow-hidden">
            <div className="px-6 py-4 border-b border-gray-200">
              <h2 className="text-lg font-semibold text-gray-900">
                Extrato - {extrato.numeroConta}
              </h2>
              <p className="text-sm text-gray-500 mt-1">
                {format(new Date(extrato.dataInicio), "dd/MM/yyyy", { locale: ptBR })} até{' '}
                {format(new Date(extrato.dataFim), "dd/MM/yyyy", { locale: ptBR })} -{' '}
                {extrato.totalTransacoes} transações
              </p>
            </div>

            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Data
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Tipo
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Valor
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Status
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Descrição
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {extrato.transacoes.length === 0 ? (
                    <tr>
                      <td colSpan={5} className="px-6 py-8 text-center text-gray-500">
                        Nenhuma transação encontrada
                      </td>
                    </tr>
                  ) : (
                    extrato.transacoes.map((transacao) => (
                      <tr key={transacao.id} className="hover:bg-gray-50">
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                          {format(new Date(transacao.dataCriacao), "dd/MM/yyyy HH:mm", {
                            locale: ptBR,
                          })}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                          {transacao.tipo}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm font-semibold text-gray-900">
                          R$ {transacao.valor.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <span
                            className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                              transacao.status === 'Processada'
                                ? 'bg-green-100 text-green-800'
                                : transacao.status === 'Falhada'
                                ? 'bg-red-100 text-red-800'
                                : 'bg-yellow-100 text-yellow-800'
                            }`}
                          >
                            {transacao.status}
                          </span>
                        </td>
                        <td className="px-6 py-4 text-sm text-gray-500">
                          {transacao.descricao || '-'}
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </div>
        ) : (
          <div className="bg-white rounded-lg shadow p-8 text-center text-gray-500">
            Selecione uma conta para ver o histórico
          </div>
        )}
      </div>
    </Layout>
  );
};

