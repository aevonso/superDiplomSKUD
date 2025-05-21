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
import RoomCard from './RoomCard'; // ← Добавь импорт
import AddAccessMatrixPage from './AddAccessMatrixPage';
import MobileDevicesPage from './MobileDevicesPage';
import MobileDeviceCard from './MobileDeviceCard';
import RegisterMobileDevicePage from './RegisterMobileDevicePage';
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
            <Route path="/devices/register" element={<RegisterMobileDevicePage />} />
            <Route path="/devices" element={<MobileDevicesPage />} />
            <Route path="/rooms/:id" element={<RoomCard />} /> {/* ← Вот это добавь */}
            <Route path="/accessmatrix/new" element={<AddAccessMatrixPage />} />
            <Route path="/devices/:id" element={<MobileDeviceCard />} />
            <Route path="*" element={<NotFound />} />
        </Routes>
    </BrowserRouter>
);
