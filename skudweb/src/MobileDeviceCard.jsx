import React, { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import Swal from 'sweetalert2';
import logo from './assets/natk-logo.png';
import phoneImg from './assets/mobPhone.png';
import {
    fetchMobileDeviceById,
    updateMobileDevice,
    deleteMobileDevice
} from './mobileDeviceApi';
import './RegisterMobileDevicePage.css';

export default function MobileDeviceCard() {
    const { id } = useParams();
    const navigate = useNavigate();
    const [device, setDevice] = useState(null);

    useEffect(() => {
        fetchMobileDeviceById(id).then(setDevice);
    }, [id]);

    const handleUpdate = async () => {
        try {
            await updateMobileDevice(id, {
                deviceName: device.deviceName,
                deviceCode: device.deviceCode
            });
            await Swal.fire({
                icon: 'success',
                title: 'Обновлено',
                timer: 1200,
                toast: true,
                position: 'top-end',
                showConfirmButton: false
            });
        } catch (err) {
            console.error(err);
            await Swal.fire({
                icon: 'error',
                title: 'Ошибка при обновлении'
            });
        }
    };

    const handleDelete = async () => {
        const confirm = await Swal.fire({
            icon: 'warning',
            title: 'Удалить устройство?',
            showCancelButton: true,
            confirmButtonText: 'Удалить',
            cancelButtonText: 'Отмена'
        });

        if (confirm.isConfirmed) {
            try {
                await deleteMobileDevice(id);
                navigate('/devices');
            } catch (err) {
                console.error(err);
                await Swal.fire({
                    icon: 'error',
                    title: 'Ошибка при удалении'
                });
            }
        }
    };

    if (!device) return <div>Загрузка...</div>;

    return (
        <div className="AddEmployee">
            <header className="Header">
                <Link to="/devices" className="BackLink">← НАТК</Link>
                <img src={logo} alt="НАТК" className="Header-logo" />
                <span className="Header-title">Карточка моб.устройства</span>
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
                                value={device.deviceName}
                                onChange={e => setDevice({ ...device, deviceName: e.target.value })}
                            />
                        </div>
                        <div className="Field">
                            <label>Сотрудник:</label>
                            <input value={device.employeeName} readOnly />
                        </div>
                        <div className="Field">
                            <label>Код устройства:</label>
                            <input
                                value={device.deviceCode}
                                onChange={e => setDevice({ ...device, deviceCode: e.target.value })}
                            />
                        </div>

                        <div className="ActionButtons">
                            <button className="Btn upload" onClick={handleUpdate}>Обновить устройство</button>
                            <button className="Btn delete" onClick={handleDelete}>Удалить устройство</button>
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
}
