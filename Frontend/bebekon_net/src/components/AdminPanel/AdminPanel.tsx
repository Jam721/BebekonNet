// src/components/AdminPanel.tsx
import { useEffect, useState } from 'react';
import styles from './AdminPanel.module.css';

interface User {
    id: string;
    userName: string;
    email: string;
    permisions: Array<{ name: string }>;
    avatarUrl: string;
}

export const AdminPanel = () => {
    const [users, setUsers] = useState<User[]>([]);
    const [isAdmin, setIsAdmin] = useState(false);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const MINIO_BASE_URL = "http://localhost:9000";

    // Проверка прав администратора
    useEffect(() => {
        const checkAdminStatus = async () => {
            try {
                const response = await fetch('http://localhost:5124/identity/User/GetRole', {
                    credentials: 'include'
                });

                if (response.ok) {
                    const role = await response.text();
                    setIsAdmin(role === 'Admin');
                }
            }
            finally {
                setLoading(false);
            }
        };
        checkAdminStatus();
    }, []);

    // Загрузка списка пользователей
    const loadUsers = async () => {
        try {
            const response = await fetch('http://localhost:5124/identity/User/AllUsers', {
                method: 'POST',
                credentials: 'include'
            });

            if (!response.ok) throw new Error('Ошибка загрузки пользователей');

            const data = await response.json();
            setUsers(data);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Ошибка загрузки');
        }
    };

    // Выдача авторства
    const grantAuthorship = async (email: string) => {
        try {
            const response = await fetch(
                `http://localhost:5124/identity/User/GiveAuthorship?email=${encodeURIComponent(email)}`,
                {
                    method: 'POST',
                    credentials: 'include'
                }
            );

            if (!response.ok) throw new Error('Ошибка выдачи прав');

            await loadUsers(); // Обновляем список после изменения
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Ошибка операции');
        }
    };

    // Первоначальная загрузка данных
    useEffect(() => {
        if (isAdmin) loadUsers();
    }, [isAdmin]);

    if (loading) return <div>Проверка прав доступа...</div>;
    if (!isAdmin) return <div>Доступ запрещен</div>;

    return (
        <div className={styles.adminContainer}>
            <h2 className={styles.adminTitle}>Административная панель</h2>

            {error && <div className={styles.errorMessage}>{error}</div>}

            <div className={styles.usersList}>
                {users.map(user => (
                    <div key={user.id} className={styles.userCard}>
                        <div className={styles.userHeader}>
                            <img
                                src={user.avatarUrl ?
                                    `${MINIO_BASE_URL}/bebekonnet/${user.avatarUrl}` :
                                    'default-avatar.png'}
                                alt="Аватар"
                                className={styles.userAvatar}
                            />
                            <div>
                                <h3>{user.userName}</h3>
                                <p>{user.email}</p>
                            </div>
                        </div>

                        <div className={styles.permissionsSection}>
                            <h4>Права:</h4>
                            <div className={styles.permissionsList}>
                                {user.permisions.map(p => (
                                    <span
                                        key={p.name}
                                        className={styles.permissionBadge}
                                    >
                                        {p.name}
                                    </span>
                                ))}
                            </div>
                        </div>

                        <button
                            onClick={() => grantAuthorship(user.email)}
                            className={styles.grantButton}
                            disabled={user.permisions.some(p => p.name === 'Create')}
                        >
                            Выдать авторство
                        </button>
                    </div>
                ))}
            </div>
        </div>
    );
};