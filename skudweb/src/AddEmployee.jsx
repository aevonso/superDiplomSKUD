import React, { useEffect, useState, useRef } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Swal from 'sweetalert2';
import PhoneInput from 'react-phone-number-input';
import 'react-phone-number-input/style.css';
import { fetchDivisions } from './divisionApi';
import { fetchPosts } from './postApi';
import { createEmployee, uploadAvatar, checkUnique } from './employeeApi';
import logo from './assets/natk-logo.png';
import './AddEmployee.css';

export default function AddEmployee() {
    const navigate = useNavigate();
    const fileInput = useRef();

    const [form, setForm] = useState({
        lastName: '',
        firstName: '',
        patronymic: '',
        email: '',
        phoneNumber: '+7',
        login: '',
        password: '',
        divisionId: '',
        postId: '',
        passportSeria: '',
        passportNumber: '',
    });
    const [divisions, setDivisions] = useState([]);
    const [posts, setPosts] = useState([]);
    const [avatarFile, setAvatarFile] = useState(null);
    const [previewUrl, setPreviewUrl] = useState(null);
    const [errors, setErrors] = useState({});

    useEffect(() => {
        fetchDivisions().then(setDivisions);
        fetchPosts().then(setPosts);
    }, []);

    const onChange = (field, value) => setForm((s) => ({ ...s, [field]: value }));

    const handleAvatarChange = (e) => {
        const file = e.target.files[0];
        if (!file) return;
        setAvatarFile(file);
        setPreviewUrl(URL.createObjectURL(file));
    };

    const handlePassportInput = (field, idx, value) => {
        if (!/^\d?$/.test(value)) return;
        const arr = (form[field] || '').padEnd(field === 'passportSeria' ? 4 : 6, ' ').split('');
        arr[idx] = value || ' ';
        onChange(field, arr.join('').trim());
    };

    const validate = async () => {
        const newErrors = {};

        if (!form.lastName) newErrors.lastName = 'Фамилия обязательна';
        if (!form.firstName) newErrors.firstName = 'Имя обязательно';
        if (!form.patronymic) newErrors.patronymic = 'Отчество обязательно';
        if (!form.email || !form.email.includes('@')) newErrors.email = 'Некорректный email';
        if (!form.login) newErrors.login = 'Логин обязателен';
        if (!form.password) newErrors.password = 'Пароль обязателен';
        if (!form.phoneNumber || form.phoneNumber.length < 12) newErrors.phoneNumber = 'Некорректный номер телефона';
        if (!form.divisionId) newErrors.divisionId = 'Подразделение обязательно';
        if (!form.postId) newErrors.postId = 'Должность обязательна';
        if (!form.passportSeria || form.passportSeria.length < 4) newErrors.passportSeria = 'Серия паспорта обязательна';
        if (!form.passportNumber || form.passportNumber.length < 6) newErrors.passportNumber = 'Номер паспорта обязателен';

        // Проверка уникальности на клиенте
        try {
            const response = await checkUnique({
                email: form.email,
                login: form.login,
                phoneNumber: form.phoneNumber,
                passportSeria: form.passportSeria,
                passportNumber: form.passportNumber,
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

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!(await validate())) {
            await Swal.fire({
                icon: 'error',
                title: 'Пожалуйста, исправьте ошибки в форме',
            });
            return;
        }

        try {
            const payload = {
                ...form,
                divisionId: Number(form.divisionId),
                postId: Number(form.postId),
            };

            const created = await createEmployee(payload);

            if (avatarFile) {
                await uploadAvatar(created.id, avatarFile);
            }

            await Swal.fire({
                icon: 'success',
                title: 'Сотрудник создан',
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 1500,
            });

            navigate('/employees');
        } catch (err) {
            const msg = err.response?.data?.message || 'Ошибка при создании';
            await Swal.fire({
                icon: 'error',
                title: msg,
            });
        }
    };

    return (
        <div className="AddEmployee">
            <header className="Header">
                <Link to="/employees" className="BackLink">
                    ← НАТК
                </Link>
                <img src={logo} alt="НАТК" className="Header-logo" />
                <span className="Header-title">Новый сотрудник</span>
            </header>

            <main className="FormMain">
                <form onSubmit={handleSubmit} className="FormBlock" autoComplete="off" noValidate>
                    <div className="AvatarSection">
                        <div className="PhotoBlock">
                            {previewUrl ? (
                                <img src={previewUrl} className="Photo" alt="preview" />
                            ) : (
                                <div className="PhotoPlaceholder">нет фото</div>
                            )}
                        </div>
                        <div className="AvatarButtons">
                            <button type="button" className="Btn upload" onClick={() => fileInput.current.click()}>
                                Загрузить фото
                            </button>
                            <button
                                type="button"
                                className="Btn delete"
                                onClick={() => {
                                    setAvatarFile(null);
                                    setPreviewUrl(null);
                                    fileInput.current.value = null;
                                }}
                            >
                                Удалить фото
                            </button>
                            <input ref={fileInput} type="file" accept="image/*" hidden onChange={handleAvatarChange} />
                        </div>
                    </div>

                    <div className="FieldsBlock">
                        <div className="FormRow">
                            <label htmlFor="lastName">Фамилия:</label>
                            <input
                                id="lastName"
                                type="text"
                                value={form.lastName}
                                required
                                onChange={(e) => onChange('lastName', e.target.value)}
                            />
                            {errors.lastName && <span className="Error">{errors.lastName}</span>}
                        </div>

                        <div className="FormRow">
                            <label htmlFor="firstName">Имя:</label>
                            <input
                                id="firstName"
                                type="text"
                                value={form.firstName}
                                required
                                onChange={(e) => onChange('firstName', e.target.value)}
                            />
                            {errors.firstName && <span className="Error">{errors.firstName}</span>}
                        </div>

                        <div className="FormRow">
                            <label htmlFor="patronymic">Отчество:</label>
                            <input
                                id="patronymic"
                                type="text"
                                value={form.patronymic}
                                required
                                onChange={(e) => onChange('patronymic', e.target.value)}
                            />
                            {errors.patronymic && <span className="Error">{errors.patronymic}</span>}
                        </div>

                        <div className="FormRow">
                            <label htmlFor="email">Email:</label>
                            <input
                                id="email"
                                type="text"
                                value={form.email}
                                required
                                onChange={(e) => onChange('email', e.target.value)}
                            />
                            {errors.email && <span className="Error">{errors.email}</span>}
                        </div>

                        <div className="FormRow">
                            <label htmlFor="login">Логин:</label>
                            <input
                                id="login"
                                type="text"
                                value={form.login}
                                required
                                onChange={(e) => onChange('login', e.target.value)}
                            />
                            {errors.login && <span className="Error">{errors.login}</span>}
                        </div>

                        <div className="FormRow">
                            <label htmlFor="password">Пароль:</label>
                            <input
                                id="password"
                                type="password"
                                value={form.password}
                                required
                                onChange={(e) => onChange('password', e.target.value)}
                            />
                            {errors.password && <span className="Error">{errors.password}</span>}
                        </div>

                        <div className="FormRow">
                            <label htmlFor="phoneNumber">Номер телефона:</label>
                            <PhoneInput
                                international
                                defaultCountry="RU"
                                value={form.phoneNumber}
                                onChange={(value) => onChange('phoneNumber', value)}
                            />
                            {errors.phoneNumber && <span className="Error">{errors.phoneNumber}</span>}
                        </div>

                        <div className="FormRow">
                            <label htmlFor="divisionId">Подразделение:</label>
                            <select
                                id="divisionId"
                                required
                                value={form.divisionId}
                                onChange={(e) => onChange('divisionId', e.target.value)}
                            >
                                <option value="">— выберите —</option>
                                {divisions.map((item) => (
                                    <option key={item.id} value={item.id}>
                                        {item.name}
                                    </option>
                                ))}
                            </select>
                            {errors.divisionId && <span className="Error">{errors.divisionId}</span>}
                        </div>

                        <div className="FormRow">
                            <label htmlFor="postId">Должность:</label>
                            <select
                                id="postId"
                                required
                                value={form.postId}
                                onChange={(e) => onChange('postId', e.target.value)}
                            >
                                <option value="">— выберите —</option>
                                {posts.map((item) => (
                                    <option key={item.id} value={item.id}>
                                        {item.name}
                                    </option>
                                ))}
                            </select>
                            {errors.postId && <span className="Error">{errors.postId}</span>}
                        </div>

                        <div className="FormRow">
                            <label>Серия и номер паспорта:</label>
                            <div className="PassportInputs">
                                <div className="PassportSeries">
                                    {[...form.passportSeria.padEnd(4, ' ')].map((ch, i) => (
                                        <input
                                            key={`seria-${i}`}
                                            maxLength={1}
                                            value={ch.trim()}
                                            onChange={(e) => handlePassportInput('passportSeria', i, e.target.value)}
                                        />
                                    ))}
                                </div>
                                <div className="PassportNumber">
                                    {[...form.passportNumber.padEnd(6, ' ')].map((ch, i) => (
                                        <input
                                            key={`number-${i}`}
                                            maxLength={1}
                                            value={ch.trim()}
                                            onChange={(e) => handlePassportInput('passportNumber', i, e.target.value)}
                                        />
                                    ))}
                                </div>
                            </div>
                            {errors.passportSeria && <span className="Error">{errors.passportSeria}</span>}
                            {errors.passportNumber && <span className="Error">{errors.passportNumber}</span>}
                        </div>

                        <div className="FormRow ActionButtons">
                            <button type="submit" className="Btn save">
                                Создать
                            </button>
                        </div>
                    </div>
                </form>
            </main>
        </div>
    );
}
