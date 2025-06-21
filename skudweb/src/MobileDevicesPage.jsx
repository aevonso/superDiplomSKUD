import React, { useEffect, useState, useRef } from 'react';
import { fetchMobileDevices } from './mobileDeviceApi';
import * as signalR from '@microsoft/signalr';
import logo from './assets/natk-logo.png';
import { useNavigate } from 'react-router-dom';
import './MobileDevicesPage.css';

export default function MobileDevicesPage() {
    const [devices, setDevices] = useState([]);
    const [loading, setLoading] = useState(false);
    const [filters, setFilters] = useState({
        date: '',
        deviceName: '',
        employeeName: '',
    });
    const navigate = useNavigate();
    const hubConnection = useRef(null);

    // Загрузка устройств с сервера
    const loadDevices = async () => {
        setLoading(true);
        try {
            const data = await fetchMobileDevices(filters);
            // фильтр по имени сотрудника на клиенте, если нужно
            const filtered = filters.employeeName
                ? data.filter(d =>
                    d.employeeName.toLowerCase().includes(filters.employeeName.toLowerCase())
                )
                : data;
            setDevices(filtered);
        } finally {
            setLoading(false);
        }
    };

    // при старте компонента — загрузка + подключение SignalR
    useEffect(() => {
        loadDevices();

        const conn = new signalR.HubConnectionBuilder()
            .withUrl('/hubs/devicestatus')
            .withAutomaticReconnect()
            .build();

        conn.on('DeviceStatusChanged', (deviceId, isActive) => {
            setDevices(current =>
                current.map(d =>
                    d.id === deviceId ? { ...d, isActive } : d
                )
            );
        });

        conn.start().catch(err => console.error('SignalR error:', err));
        hubConnection.current = conn;

        return () => {
            conn.stop();
        };
    }, []);

    return (
        <div className="Dashboard">
            <header className="Header">
                <img src={logo} alt="НАТК" className="Header-logo" />
                <span className="Header-title">НАТК</span>
            </header>

            <div className="Body">
                <aside className="Sidebar">
                    <nav>
                        <a href="/employees">Сотрудники</a>
                        <a href="/devices" className="active">Устройства</a>
                        <a href="/accessmatrix">Матрица доступа</a>
                        <a href="/dashboard">Лог событий</a>
                        <a href="/rooms">Помещения</a>
                        <a href="/divisions">Подразделения</a>
                        <a href="/posts">Должности</a>
                        <a href="/reports">Отчёты</a>
                        <a href="/setting">Настройки</a>
                    </nav>
                </aside>

                <main className="Main">
                    <div className="Filters">
                        <span>Фильтры:</span>
                        <input
                            type="date"
                            value={filters.date}
                            onChange={e => setFilters(f => ({ ...f, date: e.target.value }))}
                        />
                        <input
                            placeholder="Имя устройства"
                            value={filters.deviceName}
                            onChange={e => setFilters(f => ({ ...f, deviceName: e.target.value }))}
                        />
                        <input
                            placeholder="Сотрудник"
                            value={filters.employeeName}
                            onChange={e => setFilters(f => ({ ...f, employeeName: e.target.value }))}
                        />
                        <button onClick={loadDevices} disabled={loading}>Применить</button>
                        <button
                            onClick={() => {
                                setFilters({ date: '', deviceName: '', employeeName: '' });
                                loadDevices();
                            }}
                            disabled={loading}
                        >
                            Сбросить
                        </button>
                    </div>

                    <table className="DevicesTable">
                        <thead>
                            <tr>
                                <th>Сотрудник</th>
                                <th>Устройство</th>
                                <th>Дата регистрации</th>
                            </tr>
                        </thead>
                        <tbody>
                            {devices.map(dev => (
                                <tr
                                    key={dev.id}
                                    className="DeviceRow"
                                    onClick={() => navigate(`/devices/${dev.id}`)}
                                >
                                    <td>{dev.employeeName}</td>
                                    <td>{dev.deviceName}</td>
                                    <td>{new Date(dev.createdAt).toLocaleString('ru-RU')}</td>

                                </tr>
                            ))}
                        </tbody>
                    </table>

                    <button
                        className="AddDeviceButton"
                        onClick={() => navigate('/devices/register')}
                    >
                        Добавить устройство
                    </button>
                </main>
            </div>
        </div>
    );
}