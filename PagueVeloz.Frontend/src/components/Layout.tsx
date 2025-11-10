import { ReactNode, useState, useEffect } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import {
  LogOut,
  Wallet,
  Home,
  Settings,
  Activity,
  Plus,
  Menu,
  X,
  User,
  CreditCard,
} from 'lucide-react';

interface LayoutProps {
  children: ReactNode;
}

export const Layout = ({ children }: LayoutProps) => {
  const { logout, cliente } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [sidebarOpen, setSidebarOpen] = useState(true);
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  // Fechar menu mobile em telas maiores
  useEffect(() => {
    const handleResize = () => {
      if (window.innerWidth >= 768) {
        setMobileMenuOpen(false);
      }
    };
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const isActive = (path: string) => {
    return location.pathname === path || location.pathname.startsWith(path + '/');
  };

  const menuItems = [
    { path: '/dashboard', label: 'Dashboard', icon: Home },
    { path: '/transacoes', label: 'Transações', icon: Activity },
    { path: '/contas', label: 'Minhas Contas', icon: Wallet },
    { path: '/configuracoes', label: 'Configurações', icon: Settings },
  ];

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Mobile Header */}
      <header className="md:hidden bg-white shadow-sm border-b border-gray-200 sticky top-0 z-40">
        <div className="flex items-center justify-between px-4 h-16">
          <div className="flex items-center gap-2">
            <Wallet className="h-6 w-6 text-indigo-600" />
            <span className="text-lg font-bold text-gray-900">PagueVeloz</span>
          </div>
          <button
            onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
            className="p-2 rounded-lg hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
            aria-label="Toggle menu"
            aria-expanded={mobileMenuOpen}
          >
            {mobileMenuOpen ? (
              <X className="w-6 h-6 text-gray-600" />
            ) : (
              <Menu className="w-6 h-6 text-gray-600" />
            )}
          </button>
        </div>
      </header>

      {/* Mobile Overlay */}
      {mobileMenuOpen && (
        <div
          className="md:hidden fixed inset-0 bg-black bg-opacity-50 z-40"
          onClick={() => setMobileMenuOpen(false)}
          aria-hidden="true"
        />
      )}

      <div className="flex">
        {/* Sidebar Desktop */}
        <aside
          className={`hidden md:block ${
            sidebarOpen ? 'w-64' : 'w-20'
          } bg-white shadow-lg border-r border-gray-200 transition-all duration-300 fixed h-screen z-30`}
          aria-label="Navegação principal"
        >
          {/* Logo e Toggle */}
          <div className="flex items-center justify-between p-4 border-b border-gray-200 h-16">
            {sidebarOpen && (
              <div className="flex items-center">
                <Wallet className="h-8 w-8 text-indigo-600" aria-hidden="true" />
                <span className="ml-2 text-xl font-bold text-gray-900">PagueVeloz</span>
              </div>
            )}
            {!sidebarOpen && (
              <div className="flex justify-center w-full">
                <Wallet className="h-8 w-8 text-indigo-600" aria-label="PagueVeloz" />
              </div>
            )}
            <button
              onClick={() => setSidebarOpen(!sidebarOpen)}
              className="p-2 rounded-lg hover:bg-gray-100 transition-colors focus:outline-none focus:ring-2 focus:ring-indigo-500"
              aria-label={sidebarOpen ? 'Colapsar menu' : 'Expandir menu'}
            >
              {sidebarOpen ? (
                <X className="w-5 h-5 text-gray-600" />
              ) : (
                <Menu className="w-5 h-5 text-gray-600" />
              )}
            </button>
          </div>

          {/* Menu Items */}
          <nav className="p-4 space-y-2" aria-label="Menu principal">
            {menuItems.map((item) => {
              const Icon = item.icon;
              const active = isActive(item.path);
              return (
                <Link
                  key={item.path}
                  to={item.path}
                  className={`flex items-center gap-3 px-4 py-3 rounded-lg transition-all duration-200 ${
                    active
                      ? 'bg-indigo-50 text-indigo-600 border-l-4 border-indigo-600 font-semibold shadow-sm'
                      : 'text-gray-700 hover:bg-gray-50 hover:text-gray-900'
                  } focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2`}
                  aria-current={active ? 'page' : undefined}
                >
                  <Icon className="w-5 h-5 flex-shrink-0" aria-hidden="true" />
                  {sidebarOpen && <span className="font-medium">{item.label}</span>}
                </Link>
              );
            })}
          </nav>

          {/* Ações Rápidas */}
          {sidebarOpen && (
            <div className="p-4 border-t border-gray-200">
              <p className="text-xs font-semibold text-gray-500 uppercase mb-2 px-4 tracking-wider">
                Ações Rápidas
              </p>
              <div className="space-y-2">
                <Link
                  to="/transacoes/nova"
                  className="flex items-center gap-3 px-4 py-2.5 rounded-lg text-gray-700 hover:bg-indigo-50 hover:text-indigo-600 transition-colors focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  onClick={() => setMobileMenuOpen(false)}
                >
                  <Plus className="w-4 h-4" aria-hidden="true" />
                  <span className="text-sm font-medium">Nova Transação</span>
                </Link>
                <Link
                  to="/contas/nova"
                  className="flex items-center gap-3 px-4 py-2.5 rounded-lg text-gray-700 hover:bg-indigo-50 hover:text-indigo-600 transition-colors focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  onClick={() => setMobileMenuOpen(false)}
                >
                  <CreditCard className="w-4 h-4" aria-hidden="true" />
                  <span className="text-sm font-medium">Nova Conta</span>
                </Link>
              </div>
            </div>
          )}

          {/* User Info e Logout */}
          <div className="absolute bottom-0 left-0 right-0 p-4 border-t border-gray-200 bg-white">
            {sidebarOpen && (
              <div className="mb-3 px-4">
                <div className="flex items-center gap-3 mb-2">
                  <div className="w-10 h-10 bg-indigo-100 rounded-full flex items-center justify-center ring-2 ring-indigo-200">
                    <User className="w-5 h-5 text-indigo-600" aria-hidden="true" />
                  </div>
                  <div className="flex-1 min-w-0">
                    <p className="text-sm font-semibold text-gray-900 truncate" title={cliente?.nome}>
                      {cliente?.nome}
                    </p>
                    <p className="text-xs text-gray-500 truncate" title={cliente?.email}>
                      {cliente?.email}
                    </p>
                  </div>
                </div>
              </div>
            )}
            {!sidebarOpen && (
              <div className="flex justify-center mb-3">
                <div className="w-10 h-10 bg-indigo-100 rounded-full flex items-center justify-center ring-2 ring-indigo-200">
                  <User className="w-5 h-5 text-indigo-600" aria-label="Usuário" />
                </div>
              </div>
            )}
            <button
              onClick={handleLogout}
              className={`w-full flex items-center gap-3 px-4 py-2.5 rounded-lg text-red-600 hover:bg-red-50 transition-colors focus:outline-none focus:ring-2 focus:ring-red-500 ${
                !sidebarOpen && 'justify-center'
              }`}
              aria-label="Sair da conta"
            >
              <LogOut className="w-5 h-5 flex-shrink-0" aria-hidden="true" />
              {sidebarOpen && <span className="font-medium">Sair</span>}
            </button>
          </div>
        </aside>

        {/* Mobile Sidebar */}
        <aside
          className={`md:hidden fixed top-16 left-0 h-[calc(100vh-4rem)] w-64 bg-white shadow-xl border-r border-gray-200 transition-transform duration-300 z-50 ${
            mobileMenuOpen ? 'translate-x-0' : '-translate-x-full'
          }`}
          aria-label="Menu de navegação mobile"
        >
          <nav className="p-4 space-y-2 overflow-y-auto h-full pb-20">
            {menuItems.map((item) => {
              const Icon = item.icon;
              const active = isActive(item.path);
              return (
                <Link
                  key={item.path}
                  to={item.path}
                  onClick={() => setMobileMenuOpen(false)}
                  className={`flex items-center gap-3 px-4 py-3 rounded-lg transition-all ${
                    active
                      ? 'bg-indigo-50 text-indigo-600 border-l-4 border-indigo-600 font-semibold'
                      : 'text-gray-700 hover:bg-gray-50'
                  }`}
                  aria-current={active ? 'page' : undefined}
                >
                  <Icon className="w-5 h-5 flex-shrink-0" />
                  <span className="font-medium">{item.label}</span>
                </Link>
              );
            })}
            <div className="pt-4 border-t border-gray-200 mt-4">
              <p className="text-xs font-semibold text-gray-500 uppercase mb-2 px-4">Ações Rápidas</p>
              <Link
                to="/transacoes/nova"
                onClick={() => setMobileMenuOpen(false)}
                className="flex items-center gap-3 px-4 py-2.5 rounded-lg text-gray-700 hover:bg-indigo-50"
              >
                <Plus className="w-4 h-4" />
                <span className="text-sm font-medium">Nova Transação</span>
              </Link>
              <Link
                to="/contas/nova"
                onClick={() => setMobileMenuOpen(false)}
                className="flex items-center gap-3 px-4 py-2.5 rounded-lg text-gray-700 hover:bg-indigo-50"
              >
                <CreditCard className="w-4 h-4" />
                <span className="text-sm font-medium">Nova Conta</span>
              </Link>
            </div>
          </nav>
        </aside>

        {/* Main Content */}
        <div
          className={`flex-1 transition-all duration-300 ${
            sidebarOpen ? 'md:ml-64' : 'md:ml-20'
          }`}
        >
          <main className="min-h-screen">{children}</main>
        </div>
      </div>
    </div>
  );
};
