import React, { useState } from 'react'
import logo from './assets/natk-logo.png'

export default function App() {
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')

    const handleSubmit = e => {
        e.preventDefault()
        console.log({ email, password })
    }

    return (
        <div className="App">
            <header className="App-header">
                <img src={logo} alt="НАТК" className="App-logo" />
            </header>

            <main className="App-main">
                <form className="LoginForm" onSubmit={handleSubmit}>
                    <h1>Авторизация в систему</h1>

                    <div className="FormGroup">
                        <label htmlFor="email">Email</label>
                        <input
                            id="email"
                            type="text"
                            value={email}
                            onChange={e => setEmail(e.target.value)}
                            placeholder="Введите email"
                        />
                    </div>

                    <div className="FormGroup">
                        <label htmlFor="password">Пароль</label>
                        <input
                            id="password"
                            type="password"
                            value={password}
                            onChange={e => setPassword(e.target.value)}
                            placeholder="Введите пароль"
                        />
                    </div>

                    <button type="submit" className="SubmitButton">
                        Войти
                    </button>
                </form>
            </main>
        </div>
    )
}
