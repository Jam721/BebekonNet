// src/components/Login.tsx
import { useState } from 'react';
import { Link } from 'react-router-dom';
import styles from './Login.module.css';

export const Login = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setIsLoading(true);

        try {
            const response = await fetch('http://localhost:5124/identity/User/Login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify({ email, password })
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data.message || 'Ошибка авторизации');
            }

            // Сохраняем токен (пример для JWT)
            localStorage.setItem('authToken', data.token);

            // Перенаправляем на главную
            window.location.href = '/';
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Ошибка соединения');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className={styles.container}>
            <form className={styles.form} onSubmit={handleSubmit}>
                <h2 className={styles.title}>Вход в аккаунт</h2>

                {error && <div className={styles.error}>{error}</div>}

                <div className={styles.inputGroup}>
                    <label className={styles.label}>Email</label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        className={styles.input}
                        required
                    />
                </div>

                <div className={styles.inputGroup}>
                    <label className={styles.label}>Пароль</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        className={styles.input}
                        required
                    />
                </div>

                <button
                    type="submit"
                    className={styles.submitButton}
                    disabled={isLoading}
                >
                    {isLoading ? 'Загрузка...' : 'Войти'}
                </button>

                <p className={styles.linkText}>
                    Нет аккаунта?{' '}
                    <Link to="/register" className={styles.link}>
                        Зарегистрироваться
                    </Link>
                </p>
            </form>
        </div>
    );
};