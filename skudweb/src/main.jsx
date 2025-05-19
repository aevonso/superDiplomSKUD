import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import App from './App';
import TwoFactor from './TwoFactor';
import Dashboard from './Dashboard';
import ReportsPage from './ReportsPage';
import EmployeeList from './EmployeeList';
import EmployeeCard from './EmployeeCard';
import AddEmployee from './AddEmployee';
import NotFound from './NotFound';
import AccessMatrixPage from './AccessMatrixPage';
import DivisionList from './DivisionList';
import PostList from './PostList';
import RoomList from './RoomList';
import AddAccessMatrixPage from './AddAccessMatrixPage';
import './App.css';

ReactDOM.createRoot(document.getElementById('root')).render(
    <BrowserRouter>
        <Routes>
            <Route path="/" element={<App />} />
            <Route path="/2fa" element={<TwoFactor />} />
            <Route path="/dashboard" element={<Dashboard />} />
            <Route path="/reports" element={<ReportsPage />} />
            <Route path="/employees" element={<EmployeeList />} />
            <Route path="/employees/new" element={<AddEmployee />} />
            <Route path="/employees/:id" element={<EmployeeCard />} />
            <Route path="/accessmatrix" element={<AccessMatrixPage />} />
            <Route path="/divisions" element={<DivisionList />} />
            <Route path="/posts" element={<PostList />} />
            <Route path="/rooms" element={<RoomList />} />
            <Route path="/accessmatrix/new" element={<AddAccessMatrixPage />} />
            <Route path="*" element={<NotFound />} />
        </Routes>
    </BrowserRouter>
);
