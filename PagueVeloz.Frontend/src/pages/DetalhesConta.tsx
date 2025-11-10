import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { contaService, transacaoService } from '../services/api';
import { Layout } from '../components/Layout';
import { ArrowLeft, DollarSign, TrendingUp, Clock, History } from 'lucide-react';
import type { Conta, Saldo, Extrato } from '../types';
import { format } from 'date-fns';
import ptBR from 'date-fns/locale/pt-BR';

export const DetalhesConta = () => {
  const { id } = useParams<{ id: string }>();
  const [contaInfo, setContaInfo] = useState<Conta | null>(null);
  const [saldo, setSaldo] = useState<Saldo | null>(null);
  const [extrato, setExtrato] = useState<Extrato | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (id) {
      loadData();
    }
  }, [id]);

  const loadData = async () => {
    if (!id) return;
    try {
      const [contaData, saldoData, extratoData] = await Promise.all([
        contaService.obterConta(id),
        contaService.consultarSaldo(id),
        transacaoService.obterExtrato(id),
      ]);
      setContaInfo(contaData);
      setSaldo(saldoData);
      setExtrato(extratoData);
    } catch (error) {
      console.error('Erro ao carregar dados:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <Layout>
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="text-center text-gray-500">Carregando...</div>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <Link
          to="/contas"
          className="inline-flex items-center text-indigo-600 hover:text-indigo-700 mb-6"
        >
          <ArrowLeft className="w-4 h-4 mr-2" />
          Voltar
        </Link>

        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Detalhes da Conta</h1>
          <div className="mt-2 flex flex-wrap items-center gap-3">
            <p className="text-gray-600">
              Conta: {contaInfo?.numero ?? saldo?.numeroConta ?? '—'}
            </p>
            {contaInfo && (
              <span
                className={`px-2 py-1 text-xs font-medium rounded-full ${
                  contaInfo.status === 'Ativa'
                    ? 'bg-green-100 text-green-800'
                    : contaInfo.status === 'Bloqueada'
                    ? 'bg-yellow-100 text-yellow-800'
                    : 'bg-gray-100 text-gray-800'
                }`}
              >
                {contaInfo.status}
              </span>
            )}
          </div>
          {contaInfo && (
            <p className="text-sm text-gray-500 mt-1">
              Cliente: <span className="font-medium">{contaInfo.clienteId}</span> • Criada em{' '}
              {format(new Date(contaInfo.dataCriacao), "dd/MM/yyyy HH:mm", { locale: ptBR })}
            </p>
          )}
        </div>

        {/* Cards de Saldo */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center justify-between mb-2">
              <p className="text-sm text-gray-600">Disponível</p>
              <DollarSign className="w-5 h-5 text-green-600" />
            </div>
            <p className="text-2xl font-bold text-green-600">
              R$ {saldo?.saldoDisponivel.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
            </p>
          </div>

          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center justify-between mb-2">
              <p className="text-sm text-gray-600">Reservado</p>
              <Clock className="w-5 h-5 text-yellow-600" />
            </div>
            <p className="text-2xl font-bold text-yellow-600">
              R$ {saldo?.saldoReservado.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
            </p>
          </div>

          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center justify-between mb-2">
              <p className="text-sm text-gray-600">Total</p>
              <TrendingUp className="w-5 h-5 text-blue-600" />
            </div>
            <p className="text-2xl font-bold text-blue-600">
              R$ {saldo?.saldoTotal.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
            </p>
          </div>

          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center justify-between mb-2">
              <p className="text-sm text-gray-600">Limite de Crédito</p>
              <DollarSign className="w-5 h-5 text-indigo-600" />
            </div>
            <p className="text-2xl font-bold text-indigo-600">
              R$ {saldo?.limiteCredito.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
            </p>
          </div>
        </div>

        {/* Extrato */}
        <div className="bg-white rounded-lg shadow">
          <div className="px-6 py-4 border-b border-gray-200 flex items-center justify-between">
            <h2 className="text-xl font-semibold text-gray-900 flex items-center gap-2">
              <History className="w-5 h-5" />
              Histórico de Transações
            </h2>
            <Link
              to={`/transacoes/nova?contaId=${id}`}
              className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700"
            >
              Nova Transação
            </Link>
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
                {extrato?.transacoes.map((transacao) => (
                  <tr key={transacao.id}>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {format(new Date(transacao.dataCriacao), "dd/MM/yyyy HH:mm", { locale: ptBR })}
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
                            ? 'text-red-800'
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
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </Layout>
  );
};

