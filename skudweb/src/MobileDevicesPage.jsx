import React, { useEffect, useState } from 'react';
import { fetchMobileDevices } from './mobileDeviceApi';
import logo from './assets/natk-logo.png';
import { useNavigate } from 'react-router-dom';
import './MobileDevicesPage.css';

export default function MobileDevicesPage() {
    const [devices, setDevices] = useState([]);
    const [filters, setFilters] = useState({ date: '', employeeName: '', deviceName: '' });
    const navigate = useNavigate();

    const applyFilters = async () => {
        const data = await fetchMobileDevices(filters);
        setDevices(data);
    };

    const resetFilters = async () => {
        setFilters({ date: '', employeeName: '', deviceName: '' });
        const data = await fetchMobileDevices({});
        setDevices(data);
    };

    useEffect(() => {
        applyFilters();
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
                        <a href="/accessmatrix" >Матрица доступа</a>
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
                        <span>Фильтры: </span>
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
                        <button onClick={applyFilters}>Применить</button>
                        <button onClick={resetFilters}>Сбросить</button>
                    </div>

                    <table className="DevicesTable">
                        <thead>
                            <tr>
                                <th>Сотрудник</th>
                                <th>Имя устройства</th>
                                <th>Дата регистрации</th>
                                <th>Статус</th>
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
                                    <td>{new Date(dev.createdAt).toLocaleString()}</td>
                                    <td className={dev.isActive ? 'green' : 'red'}>
                                        {dev.isActive ? 'В сети' : 'Отключен'}
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>

                    <button className="AddDeviceButton" onClick={() => navigate('/devices/register')}>
                        Добавить устройство
                    </button>
                </main>
            </div>
        </div>
    );
}
