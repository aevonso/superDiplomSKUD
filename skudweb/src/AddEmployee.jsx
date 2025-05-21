import React, { useEffect, useState, useRef } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Swal from 'sweetalert2';
import { fetchDivisions } from './divisionApi';
import { fetchPosts } from './postApi';
import { createEmployee, uploadAvatar } from './employeeApi';
import logo from './assets/natk-logo.png';
import './AddEmployee.css';

export default function AddEmployee() {
    const navigate = useNavigate();
    const fileInput = useRef();

    const [form, setForm] = useState({
        lastName: '', firstName: '', patronymic: '',
        email: '', phoneNumber: '+7', login: '', password: '',
        divisionId: '', postId: '',
        passportSeria: '', passportNumber: ''
    });
    const [divisions, setDivisions] = useState([]);
    const [posts, setPosts] = useState([]);
    const [avatarFile, setAvatarFile] = useState(null);
    const [previewUrl, setPreviewUrl] = useState(null);

    useEffect(() => {
        fetchDivisions().then(setDivisions);
        fetchPosts().then(setPosts);
    }, []);

    const onChange = (f, v) => setForm(s => ({ ...s, [f]: v }));

    const onPhoneChange = e => {
        let v = e.target.value.replace(/\D/g, '');
        if (!v.startsWith('7')) v = '7' + v;
        onChange('phoneNumber', '+' + v);
    };

    const handleAvatarChange = e => {
        const file = e.target.files[0];
        if (!file) return;
        setAvatarFile(file);
        setPreviewUrl(URL.createObjectURL(file));
    };

    const handleSubmit = async e => {
        e.preventDefault();
        if (!form.login || !form.password) {
            await Swal.fire({
                icon: 'error',
                title: 'Логин и пароль обязательны',
            });
            return;
        }

        try {
            const payload = {
                ...form,
                divisionId: Number(form.divisionId),
                postId: Number(form.postId)
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
                timer: 1500
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
                <Link to="/employees" className="BackLink">← НАТК</Link>
                <img src={logo} alt="НАТК" className="Header-logo" />
                <span className="Header-title">Новый сотрудник</span>
            </header>

            <main className="FormMain">
                <form onSubmit={handleSubmit} className="FormBlock" autoComplete="off">
                    <div className="AvatarSection">
                        <div className="PhotoBlock">
                            {previewUrl
                                ? <img src={previewUrl} className="Photo" alt="preview" />
                                : <div className="PhotoPlaceholder">нет фото</div>
                            }
                        </div>
                        <div className="AvatarButtons">
                            <button type="button" className="Btn upload" onClick={() => fileInput.current.click()}>
                                Загрузить фото
                            </button>
                            <button type="button" className="Btn delete" onClick={() => {
                                setAvatarFile(null);
                                setPreviewUrl(null);
                                fileInput.current.value = null;
                            }}>
                                Удалить фото
                            </button>
                            <input
                                ref={fileInput}
                                type="file"
                                accept="image/*"
                                hidden
                                onChange={handleAvatarChange}
                            />
                        </div>
                    </div>

                    <div className="FieldsBlock">
                        {[['lastName', 'Фамилия'], ['firstName', 'Имя'], ['patronymic', 'Отчество'], ['email', 'Email']].map(([f, label]) => (
                            <div className="Field" key={f}>
                                <label htmlFor={f}>{label}:</label>
                                <input
                                    id={f}
                                    type={f === 'email' ? 'email' : 'text'}
                                    autoComplete="off"
                                    required
                                    value={form[f]}
                                    onChange={e => onChange(f, e.target.value)}
                                />
                            </div>
                        ))}

                        <div className="Field">
                            <label htmlFor="login">Логин:</label>
                            <input
                                id="login"
                                required
                                autoComplete="off"
                                value={form.login}
                                onChange={e => onChange('login', e.target.value)}
                            />
                        </div>
                        <div className="Field">
                            <label htmlFor="password">Пароль:</label>
                            <input
                                id="password"
                                type="password"
                                minLength={6}
                                autoComplete="new-password"
                                required
                                value={form.password}
                                onChange={e => onChange('password', e.target.value)}
                            />
                        </div>
                        <div className="Field">
                            <label htmlFor="phoneNumber">Телефон:</label>
                            <input
                                id="phoneNumber"
                                type="tel"
                                required
                                maxLength={12}
                                value={form.phoneNumber}
                                onChange={onPhoneChange}
                            />
                        </div>
                        <div className="Field">
                            <label>Подразделение:</label>
                            <select
                                required
                                value={form.divisionId}
                                onChange={e => onChange('divisionId', e.target.value)}
                            >
                                <option value="">— выберите —</option>
                                {divisions.map(d => <option key={d.id} value={d.id}>{d.name}</option>)}
                            </select>
                        </div>
                        <div className="Field">
                            <label>Должность:</label>
                            <select
                                required
                                value={form.postId}
                                onChange={e => onChange('postId', e.target.value)}
                            >
                                <option value="">— выберите —</option>
                                {posts.map(p => <option key={p.id} value={p.id}>{p.name}</option>)}
                            </select>
                        </div>

                        <div className="PassportSection">
                            <div className="PassportField">
                                <label>Серия паспорта:</label>
                                <input
                                    required
                                    maxLength={4}
                                    value={form.passportSeria}
                                    onChange={e => onChange('passportSeria', e.target.value.replace(/\D/g, '').slice(0, 4))}
                                />
                            </div>
                            <div className="PassportField">
                                <label>Номер паспорта:</label>
                                <input
                                    required
                                    maxLength={6}
                                    value={form.passportNumber}
                                    onChange={e => onChange('passportNumber', e.target.value.replace(/\D/g, '').slice(0, 6))}
                                />
                            </div>
                        </div>

                        <div className="ActionButtons">
                            <button type="submit" className="Btn save">Создать</button>
                        </div>
                    </div>
                </form>
            </main>
        </div>
    );
}
