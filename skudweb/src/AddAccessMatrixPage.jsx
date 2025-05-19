import React, { useEffect, useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import Swal from 'sweetalert2';
import {
    fetchFloors,
    fetchDivisions,
    fetchRoomsAll,
    fetchPostsAll,
    createAccessMatrixEntry
} from './accessMatrixApi';
import './AddAccessMatrixPage.css';

export default function AddAccessMatrixPage() {
    const navigate = useNavigate();
    const [floors, setFloors] = useState([]);
    const [posts, setPosts] = useState([]);
    const [rooms, setRooms] = useState([]);
    const [form, setForm] = useState({
        postId: '',
        roomId: '',
        isAccess: true
    });
    const [errors, setErrors] = useState({});

    useEffect(() => {
        fetchPostsAll().then(setPosts);
        fetchRoomsAll().then(setRooms);
    }, []);

    const onChange = (field, val) => {
        setForm(f => ({ ...f, [field]: val }));
    };

    const handleSubmit = async e => {
        e.preventDefault();
        setErrors({});

        const errs = {};
        if (!form.postId) errs.postId = 'Выберите должность';
        if (!form.roomId) errs.roomId = 'Выберите помещение';
        if (Object.keys(errs).length) {
            setErrors(errs);
            return;
        }

        try {
            await createAccessMatrixEntry({
                postId: Number(form.postId),
                roomId: Number(form.roomId),
                isAccess: form.isAccess
            });
            await Swal.fire({
                icon: 'success',
                title: 'Запись добавлена',
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 1500
            });
            navigate('/accessmatrix');
        } catch (err) {
            const msg = err.response?.data?.title || 'Ошибка при добавлении';
            Swal.fire({ icon: 'error', title: msg });
        }
    };

    return (
        <div className="AddMatrixPage">
            <header className="Header">
                <Link to="/accessmatrix" className="BackLink">← Матрица доступа</Link>
            </header>
            <form className="Form" onSubmit={handleSubmit}>
                <div className="Field">
                    <label>Должность</label>
                    <select
                        value={form.postId}
                        onChange={e => onChange('postId', e.target.value)}
                    >
                        <option value="">— выберите —</option>
                        {posts.map(p => (
                            <option key={p.id} value={p.id}>
                                {p.name} ({p.division?.name})
                            </option>
                        ))}
                    </select>
                    {errors.postId && <div className="Error">{errors.postId}</div>}
                </div>

                <div className="Field">
                    <label>Помещение</label>
                    <select
                        value={form.roomId}
                        onChange={e => onChange('roomId', e.target.value)}
                    >
                        <option value="">— выберите —</option>
                        {rooms.map(r => (
                            <option key={r.id} value={r.id}>
                                {r.name} ({r.floor?.name})
                            </option>
                        ))}
                    </select>
                    {errors.roomId && <div className="Error">{errors.roomId}</div>}
                </div>

                <div className="Field">
                    <label>Доступ</label>
                    <label className="Toggle">
                        <input
                            type="checkbox"
                            checked={form.isAccess}
                            onChange={e => onChange('isAccess', e.target.checked)}
                        />
                        <span className="Slider" />
                    </label>
                </div>

                <button type="submit" className="Btn save">
                    Добавить запись
                </button>
            </form>
        </div>
    );
}
