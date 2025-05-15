// src/EmployeeCard.jsx
import React, { useEffect, useState, useRef } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import Swal from 'sweetalert2';
import {
    fetchEmployeeById,
    updateEmployee,
    deleteEmployee,
    uploadAvatar,
    deleteAvatar,
    getAvatarUrl
} from './employeeApi';
import logo from './assets/natk-logo.png';
import './EmployeeCard.css';

export default function EmployeeCard() {
    const { id } = useParams();
    const navigate = useNavigate();
    const fileInput = useRef();

    const [emp, setEmp] = useState(null);
    const [loading, setLoading] = useState(true);
    const [previewUrl, setPreviewUrl] = useState(null);

    useEffect(() => {
        fetchEmployeeById(id)
            .then(data => {
                setEmp(data);
                setPreviewUrl(getAvatarUrl(id));
            })
            .finally(() => setLoading(false));
    }, [id]);

    if (loading) return <div>Загрузка...</div>;
    if (!emp) return <div>Сотрудник не найден</div>;

    const onPhoneChange = e => {
        let v = e.target.value.replace(/\D/g, '');
        if (!v.startsWith('7')) v = '7' + v;
        setEmp(o => ({ ...o, phoneNumber: '+' + v }));
    };

    const onPassportChange = (field, idx, digit) => {
        if (!/^\d$/.test(digit)) return;
        const arr = [...(emp[field] || '')];
        arr[idx] = digit;
        setEmp(o => ({ ...o, [field]: arr.join('') }));
    };

    const handleSave = async () => {
        try {
            await updateEmployee(id, emp);
            // После успешного сохранения переходим на список
            navigate('/employees', { state: { saved: true } });
        } catch (err) {
            Swal.fire('Ошибка', 'Не удалось сохранить данные', 'error');
        }
    };

    const handleDelete = async () => {
        const result = await Swal.fire({
            title: 'Удалить сотрудника?',
            text: 'Это действие невозможно отменить',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Да, удалить',
            cancelButtonText: 'Отмена'
        });
        if (result.isConfirmed) {
            await deleteEmployee(id);
            navigate('/employees');
        }
    };

    const handleAvatarUpload = async e => {
        const file = e.target.files[0];
        if (!file) return;
        await uploadAvatar(id, file);
        setPreviewUrl(getAvatarUrl(id) + '?t=' + Date.now());
        Swal.fire({
            icon: 'success',
            title: 'Фото загружено',
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 1500
        });
    };

    const handleAvatarDelete = async () => {
        const result = await Swal.fire({
            title: 'Удалить фото?',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Да',
            cancelButtonText: 'Нет'
        });
        if (result.isConfirmed) {
            await deleteAvatar(id);
            setPreviewUrl(null);
            fileInput.current.value = null;
            Swal.fire({
                icon: 'info',
                title: 'Фото удалено',
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 1500
            });
        }
    };

    return (
        <div className="EmployeeCard">
            <header className="Header">
                <Link to="/employees" className="BackLink">← НАТК</Link>
                <img src={logo} alt="НАТК" className="Header-logo" />
                <span className="Header-title">Карточка сотрудника</span>
            </header>

            <div className="Body">
                <main className="Main">
                    <div className="CardContent">
                        <div className="AvatarSection">
                            <div className="PhotoBlock">
                                {previewUrl
                                    ? <img src={previewUrl} className="Photo" alt="avatar" />
                                    : <div className="PhotoPlaceholder">нет фото</div>
                                }
                            </div>
                            <div className="AvatarButtons">
                                <input
                                    ref={fileInput}
                                    type="file"
                                    accept="image/*"
                                    hidden
                                    onChange={handleAvatarUpload}
                                />
                                <button
                                    type="button"
                                    className="Btn upload"
                                    onClick={() => fileInput.current.click()}
                                >
                                    Загрузить фото
                                </button>
                                <button
                                    type="button"
                                    className="Btn delete"
                                    onClick={handleAvatarDelete}
                                >
                                    Удалить фото
                                </button>
                            </div>
                        </div>

                        <div className="FieldsBlock">
                            {[
                                ['lastName', 'Фамилия'],
                                ['firstName', 'Имя'],
                                ['patronymic', 'Отчество'],
                                ['email', 'Email']
                            ].map(([f, lab]) => (
                                <div className="Field" key={f}>
                                    <label>{lab}:</label>
                                    <input
                                        type="text"
                                        value={emp[f] || ''}
                                        onChange={e => setEmp(o => ({ ...o, [f]: e.target.value }))}
                                    />
                                </div>
                            ))}

                            <div className="Field">
                                <label>Телефон:</label>
                                <input
                                    type="tel"
                                    value={emp.phoneNumber || '+7'}
                                    onChange={onPhoneChange}
                                    maxLength={12}
                                />
                            </div>

                            <div className="Field">
                                <label>Подразделение:</label>
                                <select
                                    value={emp.divisionId}
                                    onChange={e => setEmp(o => ({ ...o, divisionId: Number(e.target.value) }))}
                                >
                                    <option value={emp.divisionId}>{emp.division?.name}</option>
                                </select>
                            </div>

                            <div className="Field">
                                <label>Должность:</label>
                                <select
                                    value={emp.postId}
                                    onChange={e => setEmp(o => ({ ...o, postId: Number(e.target.value) }))}
                                >
                                    <option value={emp.postId}>{emp.post?.name}</option>
                                </select>
                            </div>

                            <div className="PassportSection">
                                <div className="PassportField">
                                    <label>Серия паспорта:</label>
                                    <div className="PassportNumbers">
                                        {Array(4).fill(0).map((_, i) => (
                                            <input
                                                key={i}
                                                maxLength={1}
                                                value={emp.passportSeria?.[i] || ''}
                                                onChange={e => onPassportChange('passportSeria', i, e.target.value)}
                                            />
                                        ))}
                                    </div>
                                </div>
                                <div className="PassportField">
                                    <label>Номер паспорта:</label>
                                    <div className="PassportNumbers">
                                        {Array(6).fill(0).map((_, i) => (
                                            <input
                                                key={i}
                                                maxLength={1}
                                                value={emp.passportNumber?.[i] || ''}
                                                onChange={e => onPassportChange('passportNumber', i, e.target.value)}
                                            />
                                        ))}
                                    </div>
                                </div>
                            </div>

                            <div className="ActionButtons">
                                <button type="button" className="Btn save" onClick={handleSave}>
                                    Сохранить
                                </button>
                                <button type="button" className="Btn delete" onClick={handleDelete}>
                                    Удалить
                                </button>
                            </div>
                        </div>
                    </div>
                </main>
            </div>
        </div>
    );
}
