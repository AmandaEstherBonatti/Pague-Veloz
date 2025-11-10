import { useEffect, useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { contaService, transacaoService } from '../services/api';
import { Layout } from '../components/Layout';
import { LoadingSpinner } from '../components/LoadingSpinner';
import { EmptyState } from '../components/EmptyState';
import { Link } from 'react-router-dom';
import {
  Wallet,
  Plus,
  ArrowRight,
  TrendingUp,
  TrendingDown,
  DollarSign,
  CreditCard,
  Clock,
  Activity,
  AlertCircle,
} from 'lucide-react';
import type { Conta, Transacao } from '../types';
import { format } from 'date-fns';
import ptBR from 'date-fns/locale/pt-BR';

export const Dashboard = () => {
  const { cliente } = useAuth();
  const [contas, setContas] = useState<Conta[]>([]);
  const [ultimasTransacoes, setUltimasTransacoes] = useState<Transacao[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (cliente) {
      loadData();
    }
  }, [cliente]);

  const loadData = async () => {
    if (!cliente) return;
    try {
      const contasData = await contaService.obterContas(cliente.id);
      setContas(contasData);

      // Carregar últimas transações de todas as contas
      if (contasData.length > 0) {
        const todasTransacoes: Transacao[] = [];
        for (const conta of contasData.slice(0, 3)) {
          try {
            const extrato = await transacaoService.obterExtrato(conta.id);
            todasTransacoes.push(...extrato.transacoes.slice(0, 5));
          } catch (error) {
            console.error(`Erro ao carregar transações da conta ${conta.id}:`, error);
          }
        }
        // Ordenar por data e pegar as 10 mais recentes
        todasTransacoes.sort(
          (a, b) => new Date(b.dataCriacao).getTime() - new Date(a.dataCriacao).getTime()
        );
        setUltimasTransacoes(todasTransacoes.slice(0, 10));
      }
    } catch (error) {
      console.error('Erro ao carregar dados:', error);
    } finally {
      setLoading(false);
    }
  };

  // Cálculos de métricas
  const saldoTotal = contas.reduce((sum, conta) => sum + conta.saldoTotal, 0);
  const saldoDisponivel = contas.reduce((sum, conta) => sum + conta.saldoDisponivel, 0);
  const saldoReservado = contas.reduce((sum, conta) => sum + conta.saldoReservado, 0);
  const limiteTotal = contas.reduce((sum, conta) => sum + conta.limiteCredito, 0);
  const limiteDisponivel = limiteTotal + saldoDisponivel;

  // Transações do mês
  const hoje = new Date();
  const primeiroDiaMes = new Date(hoje.getFullYear(), hoje.getMonth(), 1);
  const transacoesMes = ultimasTransacoes.filter(
    (t) => new Date(t.dataCriacao) >= primeiroDiaMes
  );

  const receitasMes = transacoesMes
    .filter((t) => t.tipo === 'Credito' && t.status === 'Processada')
    .reduce((sum, t) => sum + t.valor, 0);

  const despesasMes = transacoesMes
    .filter((t) => (t.tipo === 'Debito' || t.tipo === 'Transferencia') && t.status === 'Processada')
    .reduce((sum, t) => sum + t.valor, 0);

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Processada':
        return 'bg-green-100 text-green-800';
      case 'Falhada':
        return 'bg-red-100 text-red-800';
      case 'Estornada':
        return 'bg-yellow-100 text-yellow-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const getTipoIcon = (tipo: string) => {
    switch (tipo) {
      case 'Credito':
        return <TrendingUp className="w-4 h-4 text-green-600" />;
      case 'Debito':
        return <TrendingDown className="w-4 h-4 text-red-600" />;
      case 'Transferencia':
        return <ArrowRight className="w-4 h-4 text-blue-600" />;
      default:
        return <Activity className="w-4 h-4 text-gray-600" />;
    }
  };

  if (loading) {
    return (
      <Layout>
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <LoadingSpinner size="lg" text="Carregando informações..." />
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Dashboard Financeiro</h1>
          <p className="text-gray-600 mt-2">
            Bem-vindo, <span className="font-semibold">{cliente?.nome}</span>
          </p>
        </div>

        {/* Cards de Métricas Principais */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          {/* Saldo Total */}
          <div className="bg-gradient-to-br from-indigo-500 to-indigo-600 rounded-lg shadow-lg p-6 text-white">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-sm font-medium opacity-90">Saldo Total</h3>
              <Wallet className="w-8 h-8 opacity-80" />
            </div>
            <p className="text-3xl font-bold">
              R$ {saldoTotal.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
            </p>
            <p className="text-sm opacity-75 mt-2">
              {contas.length} {contas.length === 1 ? 'conta' : 'contas'}
            </p>
          </div>

          {/* Saldo Disponível */}
          <div className="bg-white rounded-lg shadow p-6 border-l-4 border-green-500">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-sm font-medium text-gray-600">Disponível</h3>
              <DollarSign className="w-6 h-6 text-green-600" />
            </div>
            <p className="text-2xl font-bold text-gray-900">
              R$ {saldoDisponivel.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
            </p>
            <p className="text-xs text-gray-500 mt-2">Para uso imediato</p>
          </div>

          {/* Saldo Reservado */}
          <div className="bg-white rounded-lg shadow p-6 border-l-4 border-yellow-500">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-sm font-medium text-gray-600">Reservado</h3>
              <Clock className="w-6 h-6 text-yellow-600" />
            </div>
            <p className="text-2xl font-bold text-gray-900">
              R$ {saldoReservado.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
            </p>
            <p className="text-xs text-gray-500 mt-2">Aguardando captura</p>
          </div>

          {/* Limite Disponível */}
          <div className="bg-white rounded-lg shadow p-6 border-l-4 border-blue-500">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-sm font-medium text-gray-600">Limite Disponível</h3>
              <CreditCard className="w-6 h-6 text-blue-600" />
            </div>
            <p className="text-2xl font-bold text-gray-900">
              R$ {limiteDisponivel.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
            </p>
            <p className="text-xs text-gray-500 mt-2">Crédito + Saldo</p>
          </div>
        </div>

        {/* Resumo do Mês */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center justify-between mb-2">
              <h3 className="text-sm font-medium text-gray-600">Receitas do Mês</h3>
              <TrendingUp className="w-5 h-5 text-green-600" />
            </div>
            <p className="text-2xl font-bold text-green-600">
              R$ {receitasMes.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
            </p>
            <p className="text-xs text-gray-500 mt-1">
              {transacoesMes.filter((t) => t.tipo === 'Credito').length} transações
            </p>
          </div>

          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center justify-between mb-2">
              <h3 className="text-sm font-medium text-gray-600">Despesas do Mês</h3>
              <TrendingDown className="w-5 h-5 text-red-600" />
            </div>
            <p className="text-2xl font-bold text-red-600">
              R$ {despesasMes.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
            </p>
            <p className="text-xs text-gray-500 mt-1">
              {transacoesMes.filter((t) => t.tipo === 'Debito' || t.tipo === 'Transferencia').length}{' '}
              transações
            </p>
          </div>

          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex items-center justify-between mb-2">
              <h3 className="text-sm font-medium text-gray-600">Saldo do Mês</h3>
              <Activity className="w-5 h-5 text-indigo-600" />
            </div>
            <p
              className={`text-2xl font-bold ${
                receitasMes - despesasMes >= 0 ? 'text-green-600' : 'text-red-600'
              }`}
            >
              R$ {(receitasMes - despesasMes).toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
            </p>
            <p className="text-xs text-gray-500 mt-1">
              {format(new Date(), "MMMM yyyy", { locale: ptBR })}
            </p>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          {/* Minhas Contas */}
          <div className="bg-white rounded-lg shadow">
            <div className="px-6 py-4 border-b border-gray-200 flex items-center justify-between">
              <h2 className="text-xl font-semibold text-gray-900 flex items-center gap-2">
                <Wallet className="w-5 h-5" />
                Minhas Contas
              </h2>
              <Link
                to="/contas/nova"
                className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 flex items-center gap-2 text-sm"
              >
                <Plus className="w-4 h-4" />
                Nova Conta
              </Link>
            </div>

            {contas.length === 0 ? (
              <EmptyState
                icon={<Wallet className="w-16 h-16" />}
                title="Nenhuma conta encontrada"
                description="Crie sua primeira conta para começar a gerenciar suas finanças"
                action={
                  <Link
                    to="/contas/nova"
                    className="inline-flex items-center gap-2 px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors"
                  >
                    <Plus className="w-4 h-4" />
                    Criar primeira conta
                  </Link>
                }
              />
            ) : (
              <div className="divide-y divide-gray-200">
                {contas.map((conta) => (
                  <Link
                    key={conta.id}
                    to={`/contas/${conta.id}`}
                    className="block p-6 hover:bg-gray-50 transition-colors"
                  >
                    <div className="flex items-center justify-between">
                      <div className="flex-1">
                        <div className="flex items-center gap-3 mb-2">
                          <h3 className="text-lg font-semibold text-gray-900">{conta.numero}</h3>
                          <span
                            className={`px-2 py-1 text-xs font-medium rounded-full ${
                              conta.status === 'Ativa'
                                ? 'bg-green-100 text-green-800'
                                : 'bg-gray-100 text-gray-800'
                            }`}
                          >
                            {conta.status}
                          </span>
                        </div>
                        <div className="grid grid-cols-3 gap-4 text-sm">
                          <div>
                            <p className="text-gray-600">Disponível</p>
                            <p className="font-semibold text-green-600">
                              R$ {conta.saldoDisponivel.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                            </p>
                          </div>
                          <div>
                            <p className="text-gray-600">Reservado</p>
                            <p className="font-semibold text-yellow-600">
                              R$ {conta.saldoReservado.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                            </p>
                          </div>
                          <div>
                            <p className="text-gray-600">Total</p>
                            <p className="font-semibold text-gray-900">
                              R$ {conta.saldoTotal.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                            </p>
                          </div>
                        </div>
                      </div>
                      <ArrowRight className="w-6 h-6 text-gray-400" />
                    </div>
                  </Link>
                ))}
              </div>
            )}
          </div>

          {/* Últimas Transações */}
          <div className="bg-white rounded-lg shadow">
            <div className="px-6 py-4 border-b border-gray-200 flex items-center justify-between">
              <h2 className="text-xl font-semibold text-gray-900 flex items-center gap-2">
                <Activity className="w-5 h-5" />
                Últimas Transações
              </h2>
              <Link
                to="/transacoes"
                className="text-indigo-600 hover:text-indigo-700 text-sm font-medium"
              >
                Ver todas
              </Link>
            </div>

            {ultimasTransacoes.length === 0 ? (
              <EmptyState
                icon={<Activity className="w-16 h-16" />}
                title="Nenhuma transação encontrada"
                description="Suas transações aparecerão aqui quando você começar a usá-las"
                action={
                  <Link
                    to="/transacoes/nova"
                    className="inline-flex items-center gap-2 px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors"
                  >
                    <Plus className="w-4 h-4" />
                    Criar primeira transação
                  </Link>
                }
              />
            ) : (
              <div className="divide-y divide-gray-200 max-h-96 overflow-y-auto">
                {ultimasTransacoes.map((transacao) => (
                  <Link
                    key={transacao.id}
                    to={`/contas/${transacao.contaId}`}
                    className="block p-4 hover:bg-gray-50 transition-colors border-l-4 border-transparent hover:border-indigo-500"
                  >
                    <div className="flex items-start justify-between">
                      <div className="flex items-start gap-3 flex-1">
                        <div className="mt-1">{getTipoIcon(transacao.tipo)}</div>
                        <div className="flex-1 min-w-0">
                          <div className="flex items-center gap-2 mb-1">
                            <p className="text-sm font-medium text-gray-900">{transacao.tipo}</p>
                            <span
                              className={`px-2 py-0.5 text-xs font-medium rounded-full ${getStatusColor(
                                transacao.status
                              )}`}
                            >
                              {transacao.status}
                            </span>
                          </div>
                          <p className="text-xs text-gray-500 truncate">
                            {transacao.descricao || 'Sem descrição'}
                          </p>
                          <p className="text-xs text-gray-400 mt-1">
                            {format(new Date(transacao.dataCriacao), "dd/MM/yyyy HH:mm", {
                              locale: ptBR,
                            })}
                          </p>
                        </div>
                      </div>
                      <div className="text-right ml-4">
                        <p
                          className={`text-sm font-semibold ${
                            transacao.tipo === 'Credito'
                              ? 'text-green-600'
                              : transacao.tipo === 'Debito' || transacao.tipo === 'Transferencia'
                              ? 'text-red-600'
                              : 'text-gray-600'
                          }`}
                        >
                          {transacao.tipo === 'Credito' ? '+' : '-'}R${' '}
                          {transacao.valor.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                        </p>
                        {transacao.erro && (
                          <div className="flex items-center gap-1 mt-1 justify-end">
                            <AlertCircle className="w-3 h-3 text-red-500" />
                            <p className="text-xs text-red-500 truncate max-w-[100px]" title={transacao.erro}>
                              {transacao.erro}
                            </p>
                          </div>
                        )}
                      </div>
                    </div>
                  </Link>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Ações Rápidas */}
        <div className="mt-8 bg-white rounded-lg shadow p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Ações Rápidas</h2>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            <Link
              to="/transacoes/nova"
              className="flex flex-col items-center p-4 border-2 border-gray-200 rounded-lg hover:border-indigo-500 hover:bg-indigo-50 transition-all focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
              aria-label="Criar nova transação"
            >
              <Plus className="w-8 h-8 text-indigo-600 mb-2" />
              <span className="text-sm font-medium text-gray-700">Nova Transação</span>
            </Link>
            <Link
              to="/contas/nova"
              className="flex flex-col items-center p-4 border-2 border-gray-200 rounded-lg hover:border-indigo-500 hover:bg-indigo-50 transition-all focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
              aria-label="Criar nova conta"
            >
              <Wallet className="w-8 h-8 text-indigo-600 mb-2" />
              <span className="text-sm font-medium text-gray-700">Nova Conta</span>
            </Link>
            <Link
              to="/transacoes"
              className="flex flex-col items-center p-4 border-2 border-gray-200 rounded-lg hover:border-indigo-500 hover:bg-indigo-50 transition-all focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
              aria-label="Ver histórico de transações"
            >
              <Activity className="w-8 h-8 text-indigo-600 mb-2" />
              <span className="text-sm font-medium text-gray-700">Ver Histórico</span>
            </Link>
            <Link
              to="/configuracoes"
              className="flex flex-col items-center p-4 border-2 border-gray-200 rounded-lg hover:border-indigo-500 hover:bg-indigo-50 transition-all focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
              aria-label="Acessar configurações"
            >
              <TrendingUp className="w-8 h-8 text-indigo-600 mb-2" />
              <span className="text-sm font-medium text-gray-700">Configurações</span>
            </Link>
          </div>
        </div>
      </div>
    </Layout>
  );
};

