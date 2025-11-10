import { Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { ToastProvider } from './contexts/ToastContext';
import { ProtectedRoute } from './components/ProtectedRoute';
import { Login } from './pages/Login';
import { Cadastro } from './pages/Cadastro';
import { Dashboard } from './pages/Dashboard';
import { Contas } from './pages/Contas';
import { DetalhesConta } from './pages/DetalhesConta';
import { NovaConta } from './pages/NovaConta';
import { Transacoes } from './pages/Transacoes';
import { NovaTransacao } from './pages/NovaTransacao';
import { Configuracoes } from './pages/Configuracoes';

function App() {
  return (
    <AuthProvider>
      <ToastProvider>
        <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/cadastro" element={<Cadastro />} />
        <Route
          path="/dashboard"
          element={
            <ProtectedRoute>
              <Dashboard />
            </ProtectedRoute>
          }
        />
        <Route
          path="/contas"
          element={
            <ProtectedRoute>
              <Contas />
            </ProtectedRoute>
          }
        />
        <Route
          path="/contas/:id"
          element={
            <ProtectedRoute>
              <DetalhesConta />
            </ProtectedRoute>
          }
        />
        <Route
          path="/contas/nova"
          element={
            <ProtectedRoute>
              <NovaConta />
            </ProtectedRoute>
          }
        />
        <Route
          path="/transacoes"
          element={
            <ProtectedRoute>
              <Transacoes />
            </ProtectedRoute>
          }
        />
        <Route
          path="/transacoes/nova"
          element={
            <ProtectedRoute>
              <NovaTransacao />
            </ProtectedRoute>
          }
        />
        <Route
          path="/configuracoes"
          element={
            <ProtectedRoute>
              <Configuracoes />
            </ProtectedRoute>
          }
        />
          <Route path="/" element={<Navigate to="/dashboard" replace />} />
        </Routes>
      </ToastProvider>
    </AuthProvider>
  );
}

export default App;

