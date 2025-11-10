import { useEffect, useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { contaService } from '../services/api';
import { Layout } from '../components/Layout';
import { Link } from 'react-router-dom';
import { Wallet, Plus, ArrowRight } from 'lucide-react';
import type { Conta } from '../types';

export const Contas = () => {
  const { cliente } = useAuth();
  const [contas, setContas] = useState<Conta[]>([]);
  const [loading, setLoading] = useState(true);

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
        <div className="mb-8 flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Minhas Contas</h1>
            <p className="text-gray-600 mt-2">Gerencie todas as suas contas financeiras</p>
          </div>
          <Link
            to="/contas/nova"
            className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 flex items-center gap-2"
          >
            <Plus className="w-5 h-5" />
            Nova Conta
          </Link>
        </div>

        {contas.length === 0 ? (
          <div className="bg-white rounded-lg shadow p-12 text-center">
            <Wallet className="w-16 h-16 text-gray-400 mx-auto mb-4" />
            <p className="text-gray-500 mb-4">Você ainda não tem contas</p>
            <Link
              to="/contas/nova"
              className="text-indigo-600 hover:text-indigo-700 font-medium"
            >
              Criar primeira conta
            </Link>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {contas.map((conta) => (
              <Link
                key={conta.id}
                to={`/contas/${conta.id}`}
                className="bg-white rounded-lg shadow p-6 hover:shadow-lg transition-shadow"
              >
                <div className="flex items-center justify-between mb-4">
                  <div className="flex items-center gap-3">
                    <div className="w-12 h-12 bg-indigo-100 rounded-lg flex items-center justify-center">
                      <Wallet className="w-6 h-6 text-indigo-600" />
                    </div>
                    <div>
                      <h3 className="font-semibold text-gray-900">{conta.numero}</h3>
                      <span
                        className={`text-xs px-2 py-1 rounded-full ${
                          conta.status === 'Ativa'
                            ? 'bg-green-100 text-green-800'
                            : 'bg-gray-100 text-gray-800'
                        }`}
                      >
                        {conta.status}
                      </span>
                    </div>
                  </div>
                  <ArrowRight className="w-5 h-5 text-gray-400" />
                </div>

                <div className="space-y-2">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600">Disponível</span>
                    <span className="font-semibold text-green-600">
                      R$ {conta.saldoDisponivel.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                    </span>
                  </div>
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600">Reservado</span>
                    <span className="font-semibold text-yellow-600">
                      R$ {conta.saldoReservado.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                    </span>
                  </div>
                  <div className="flex justify-between text-sm pt-2 border-t border-gray-200">
                    <span className="text-gray-900 font-medium">Total</span>
                    <span className="font-bold text-gray-900">
                      R$ {conta.saldoTotal.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                    </span>
                  </div>
                </div>
              </Link>
            ))}
          </div>
        )}
      </div>
    </Layout>
  );
};

