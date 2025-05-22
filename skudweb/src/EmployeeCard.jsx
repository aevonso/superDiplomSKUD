import React, { useEffect, useState, useRef } from 'react';
import { useParams, Link, useNavigate, useLocation } from 'react-router-dom';
import Swal from 'sweetalert2';
import {
    fetchEmployeeById,
    updateEmployee,
    deleteEmployee,
    uploadAvatar,
    deleteAvatar,
    getAvatarUrl,
    checkUnique
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
    const [errors, setErrors] = useState({});

    const [divisions, setDivisions] = useState([]);
    const [posts, setPosts] = useState([]);

    useEffect(() => {
        fetchDivisions().then(setDivisions);
        fetchPosts().then(setPosts);
    }, []);

    useEffect(() => {
        fetchEmployeeById(id)
            .then(data => {
                setEmp(data);
                setPreviewUrl(getAvatarUrl(id));
            })
            .finally(() => setLoading(false));
    }, [id]);

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

    const validate = async () => {
        const newErrors = {};

        if (!emp.lastName) newErrors.lastName = 'Фамилия обязательна';
        if (!emp.firstName) newErrors.firstName = 'Имя обязательно';
        if (!emp.patronymic) newErrors.patronymic = 'Отчество обязательно';
        if (!emp.email || !emp.email.includes('@')) newErrors.email = 'Некорректный email';
        if (!emp.login) newErrors.login = 'Логин обязателен';
        if (!emp.phoneNumber || emp.phoneNumber.length < 12) newErrors.phoneNumber = 'Некорректный номер телефона';
        if (!emp.divisionId) newErrors.divisionId = 'Подразделение обязательно';
        if (!emp.postId) newErrors.postId = 'Должность обязательна';
        if (!emp.passportSeria || emp.passportSeria.length < 4) newErrors.passportSeria = 'Серия паспорта обязательна';
        if (!emp.passportNumber || emp.passportNumber.length < 6) newErrors.passportNumber = 'Номер паспорта обязателен';

        // Проверка уникальности на клиенте
        try {
            const response = await checkUnique({
                email: emp.email,
                login: emp.login,
                phoneNumber: emp.phoneNumber,
                passportSeria: emp.passportSeria,
                passportNumber: emp.passportNumber,
            });

            if (response.emailExists) newErrors.email = 'Email уже используется';
            if (response.loginExists) newErrors.login = 'Логин уже используется';
            if (response.phoneNumberExists) newErrors.phoneNumber = 'Номер телефона уже используется';
            if (response.passportExists) newErrors.passport = 'Паспорт уже зарегистрирован';
        } catch (error) {
            console.error('Ошибка при проверке уникальности:', error);
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSave = async () => {
        if (!(await validate())) {
            await Swal.fire({
                icon: 'error',
                title: 'Пожалуйста, исправьте ошибки в форме',
            });
            return;
        }

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

            <main className="FormMain">
                <div className="CardContent">
                    <div className="AvatarSection">
                        <div className="PhotoBlock">
                            {previewUrl ? (
                                <img src={previewUrl} className="Photo" alt="avatar" />
                            ) : (
                                <div className="PhotoPlaceholder">нет фото</div>
                            )}
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
                        <div className="FormRow">
                            <label htmlFor="lastName">Фамилия:</label>
                            <input
                                id="lastName"
                                type="text"
                                value={emp.lastName || ''}
                                onChange={e => setEmp(o => ({ ...o, lastName: e.target.value }))}
                            />
                            {errors.lastName && <span className="Error">{errors.lastName}</span>}
                        </div>

                        <div className="FormRow">
                            <label htmlFor="firstName">Имя:</label>
                            <input
                                id="firstName"
                                type="text"
                                value={emp.firstName || ''}
                                onChange={e => setEmp(o => ({ ...o, firstName: e.target.value }))}
                            />
                            {errors.firstName && <span className="Error">{errors.firstName}</span>}
                        </div>

                        <div className="FormRow">
                            <label htmlFor="patronymic">Отчество:</label>
                            <input
                                id="patronymic"
                                type="text"
                                value={emp.patronymic || ''}
                                onChange={e => setEmp(o => ({ ...o, patronymic: e.target.value }))}
                            />
                            {errors.patronymic && <span className="Error">{errors.patronymic}</span>}
                        </div>

                        <div className="FormRow">
                            <label htmlFor="email">Email:</label>
                            <input
                                id="email"
                                type="email"
                                value={emp.email || ''}
                                onChange={e => setEmp(o => ({ ...o, email: e.target.value }))}
                            />
                            {errors.email && <span className="Error">{errors.email}</span>}
                        </div>

                        <div className="FormRow">
                            <label htmlFor="phoneNumber">Телефон:</label>
                            <input
                                id="phoneNumber"
                                type="tel"
                                value={emp.phoneNumber || '+7'}
                                onChange={onPhoneChange}
                            />
                            {errors.phoneNumber && <span className="Error">{errors.phoneNumber}</span>}
                        </div>

                        <div className="FormRow">
                            <label htmlFor="divisionId">Подразделение:</label>
                            <select
                                id="divisionId"
                                value={emp.divisionId || ''}
                                onChange={e => setEmp(o => ({ ...o, divisionId: Number(e.target.value) }))}
                            >
                                <option value="">— выберите —</option>
                                {divisions.map(d => (
                                    <option key={d.id} value={d.id}>{d.name}</option>
                                ))}
                            </select>
                            {errors.divisionId && <span className="Error">{errors.divisionId}</span>}
                        </div>

                        <div className="FormRow">
                            <label htmlFor="postId">Должность:</label>
                            <select
                                id="postId"
                                value={emp.postId || ''}
                                onChange={e => setEmp(o => ({ ...o, postId: Number(e.target.value) }))}
                            >
                                <option value="">— выберите —</option>
                                {posts.map(p => (
                                    <option key={p.id} value={p.id}>{p.name}</option>
                                ))}
                            </select>
                            {errors.postId && <span className="Error">{errors.postId}</span>}
                        </div>

                        <div className="FormRow">
                            <label>Серия и номер паспорта:</label>
                            <div className="PassportInputs">
                                <div className="PassportSeries">
                                    {Array(4).fill(0).map((_, i) => (
                                        <input
                                            key={`seria-${i}`}
                                            maxLength={1}
                                            value={emp.passportSeria?.[i] || ''}
                                            onChange={e => onPassportChange('passportSeria', i, e.target.value)}
                                        />
                                    ))}
                                </div>
                                <div className="PassportNumber">
                                    {Array(6).fill(0).map((_, i) => (
                                        <input
                                            key={`number-${i}`}
                                            maxLength={1}
                                            value={emp.passportNumber?.[i] || ''}
                                            onChange={e => onPassportChange('passportNumber', i, e.target.value)}
                                        />
                                    ))}
                                </div>
                            </div>
                            {errors.passportSeria && <span className="Error">{errors.passportSeria}</span>}
                            {errors.passportNumber && <span className="Error">{errors.passportNumber}</span>}
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
    );
}
