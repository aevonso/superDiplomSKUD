import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Swal from 'sweetalert2';
import { fetchEmployees } from './employeeApi';
import { addMobileDevice } from './mobileDeviceApi';
import logo from './assets/natk-logo.png';
import phoneImg from './assets/mobPhone.png';
import './RegisterMobileDevicePage.css';

export default function RegisterMobileDevicePage() {
    const navigate = useNavigate();
    const [deviceName, setDeviceName] = useState('');
    const [deviceCode, setDeviceCode] = useState('');
    const [employerId, setEmployerId] = useState('');
    const [employees, setEmployees] = useState([]);

    useEffect(() => {
        fetchEmployees().then(setEmployees);
    }, []);

    const generateDeviceCode = () => {
        // Генерация случайного 6-значного кода
        const randomCode = Math.floor(100000 + Math.random() * 900000).toString();
        setDeviceCode(randomCode); // Сохранение сгенерированного кода
    };

    const handleSubmit = async () => {
        if (!deviceName || !deviceCode || !employerId) {
            await Swal.fire({
                icon: 'warning',
                title: 'Заполните все поля',
                confirmButtonText: 'Ок'
            });
            return;
        }
        try {
            await addMobileDevice({
                employerId: Number(employerId),
                deviceName,
                deviceCode
            });
            await Swal.fire({
                icon: 'success',
                title: 'Устройство добавлено!',
                timer: 1500,
                toast: true,
                position: 'top-end',
                showConfirmButton: false
            });
            navigate('/devices');
        } catch (err) {
            console.error(err);
            await Swal.fire({
                icon: 'error',
                title: 'Ошибка при добавлении устройства',
                text: err?.response?.data?.message || 'Проверьте данные и попробуйте снова'
            });
        }
    };

    return (
        <div className="AddEmployee">
            <header className="Header">
                <Link to="/devices" className="BackLink">← НАТК</Link>
                <img src={logo} alt="НАТК" className="Header-logo" />
                <span className="Header-title">Регистрация моб.устройства</span>
            </header>

            <main className="FormMain">
                <div className="FormBlock">
                    <div className="PhotoBlock">
                        <img src={phoneImg} alt="Устройство" className="Photo" />
                    </div>

                    <div className="FieldsBlock">
                        <div className="Field">
                            <label>Имя устройства:</label>
                            <input
                                value={deviceName}
                                onChange={e => setDeviceName(e.target.value)}
                                required
                            />
                        </div>

                        <div className="Field">
                            <label>Сотрудник:</label>
                            <select
                                value={employerId}
                                onChange={e => setEmployerId(e.target.value)}
                                required
                            >
                                <option value="">— выберите —</option>
                                {employees.map(e => (
                                    <option key={e.id} value={e.id}>
                                        {e.lastName} {e.firstName[0]}.{e.patronymic?.[0] ?? ''}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div className="Field">
                            <label>Код устройства:</label>
                            <input
                                value={deviceCode}
                                onChange={e => setDeviceCode(e.target.value)}
                                required
                                readOnly
                            />
                        </div>

                        <div className="ActionButtons">
                            <button
                                type="button"
                                className="Btn"
                                onClick={generateDeviceCode}  // Генерация кода
                            >
                                Сгенерировать код
                            </button>
                            <button className="Btn save" onClick={handleSubmit}>
                                Добавить устройство
                            </button>
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
}
