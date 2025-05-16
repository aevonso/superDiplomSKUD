// src/EmployeeCard.jsx
import React, { useEffect, useState, useRef } from 'react';
import { useParams, Link, useNavigate, useLocation } from 'react-router-dom';
import Swal from 'sweetalert2';
import {
    fetchEmployeeById,
    updateEmployee,
    deleteEmployee,
    uploadAvatar,
    deleteAvatar,
    getAvatarUrl
} from './employeeApi';
import { fetchDivisions } from './divisionApi';
import { fetchPosts } from './postApi';
import logo from './assets/natk-logo.png';
import './EmployeeCard.css';

export default function EmployeeCard() {
    const { id } = useParams();
    const navigate = useNavigate();
    const location = useLocation();
    const fileInput = useRef();

    const [emp, setEmp] = useState(null);
    const [loading, setLoading] = useState(true);
    const [previewUrl, setPreviewUrl] = useState(null);

    const [divisions, setDivisions] = useState([]);
    const [posts, setPosts] = useState([]);

    // 1) При монтировании подгружаем справочники подразделений и должностей
    useEffect(() => {
        fetchDivisions().then(setDivisions);
        fetchPosts().then(setPosts);
    }, []);

    // 2) При монтировании — загрузка конкретного сотрудника
    useEffect(() => {
        fetchEmployeeById(id)
            .then(data => {
                setEmp(data);
                setPreviewUrl(getAvatarUrl(id));
            })
            .finally(() => setLoading(false));
    }, [id]);

    // 3) Показать «сохранено» если пришло из списка
    useEffect(() => {
        if (location.state?.saved) {
            Swal.fire({
                icon: 'success',
                title: 'Сохранено',
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 1500
            });
            navigate(location.pathname, { replace: true, state: {} });
        }
    }, [location, navigate]);

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
            navigate('/employees', { state: { saved: true } });
        } catch (err) {
            const msg = err.response?.data?.message || 'Не удалось сохранить';
            await Swal.fire({ icon: 'error', title: msg, confirmButtonText: 'OK' });
        }
    };

    const handleDelete = async () => {
        const res = await Swal.fire({
            icon: 'warning',
            title: 'Удалить сотрудника?',
            text: 'Это действие невозможно отменить',
            showCancelButton: true,
            confirmButtonText: 'Да, удалить',
            cancelButtonText: 'Отмена'
        });
        if (res.isConfirmed) {
            await deleteEmployee(id);
            navigate('/employees');
        }
    };

    const handleAvatarUpload = async e => {
        const file = e.target.files[0];
        if (!file) return;
        await uploadAvatar(id, file);
        setPreviewUrl(getAvatarUrl(id) + '?t=' + Date.now());
        Swal.fire({ icon: 'success', title: 'Фото загружено', toast: true, position: 'top-end', showConfirmButton: false, timer: 1500 });
    };

    const handleAvatarDelete = async () => {
        const res = await Swal.fire({
            icon: 'question',
            title: 'Удалить фото?',
            showCancelButton: true,
            confirmButtonText: 'Да',
            cancelButtonText: 'Нет'
        });
        if (res.isConfirmed) {
            await deleteAvatar(id);
            setPreviewUrl(null);
            fileInput.current.value = null;
            Swal.fire({ icon: 'info', title: 'Фото удалено', toast: true, position: 'top-end', showConfirmButton: false, timer: 1500 });
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

                        {/* Фото */}
                        <div className="AvatarSection">
                            <div className="PhotoBlock">
                                {previewUrl
                                    ? <img src={previewUrl} className="Photo" alt="avatar" />
                                    : <div className="PhotoPlaceholder">нет фото</div>}
                            </div>
                            <div className="AvatarButtons">
                                <input ref={fileInput} type="file" accept="image/*" hidden onChange={handleAvatarUpload} />
                                <button type="button" className="Btn upload" onClick={() => fileInput.current.click()}>Загрузить фото</button>
                                <button type="button" className="Btn delete" onClick={handleAvatarDelete}>Удалить фото</button>
                            </div>
                        </div>

                        {/* Поля */}
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
                                        type={f === 'email' ? 'email' : 'text'}
                                        value={emp[f] || ''}
                                        onChange={e => setEmp(o => ({ ...o, [f]: e.target.value }))}
                                    />
                                </div>
                            ))}

                            <div className="Field">
                                <label>Телефон:</label>
                                <input type="tel" maxLength={12} value={emp.phoneNumber || '+7'} onChange={onPhoneChange} />
                            </div>

                            <div className="Field">
                                <label>Подразделение:</label>
                                <select
                                    value={emp.divisionId}
                                    onChange={e => setEmp(o => ({ ...o, divisionId: Number(e.target.value) }))}
                                >
                                    <option value="">— выберите —</option>
                                    {divisions.map(d => (
                                        <option key={d.id} value={d.id}>{d.name}</option>
                                    ))}
                                </select>
                            </div>

                            <div className="Field">
                                <label>Должность:</label>
                                <select
                                    value={emp.postId}
                                    onChange={e => setEmp(o => ({ ...o, postId: Number(e.target.value) }))}
                                >
                                    <option value="">— выберите —</option>
                                    {posts
                                        .filter(p => p.divisionId === emp.divisionId) // опционально: показать только из текущего отдела
                                        .map(p => (
                                            <option key={p.id} value={p.id}>{p.name}</option>
                                        ))}
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
                                <button type="button" className="Btn save" onClick={handleSave}>Сохранить</button>
                                <button type="button" className="Btn delete" onClick={handleDelete}>Удалить</button>
                            </div>
                        </div>
                    </div>
                </main>
            </div>
        </div>
    );
}
