// src/components/Header.tsx
import { useState, useEffect } from 'react';
import styles from './Header.module.css';
import logo from '../../assets/646.png';
import { Link, useNavigate } from "react-router-dom";

interface UserData {
    userName: string;
    email: string;
    avatarUrl: string;
    permisions: Array<{ name: string }>;
    createdAt: string;
}

export const Header = () => {
    const [isMenuOpen, setIsMenuOpen] = useState(false);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [userData, setUserData] = useState<UserData | null>(null);
    const navigate = useNavigate();
    const MINIO_BASE_URL = "http://localhost:9000";

    useEffect(() => {
        const checkAuth = async () => {
            try {
                const response = await fetch('http://localhost:5124/identity/User/Me', {
                    credentials: 'include'
                });

                if (response.ok) {
                    const data = await response.json();
                    setUserData({
                        ...data,
                        // Правильное объединение URL
                        avatarUrl: data.avatarUrl ? `${MINIO_BASE_URL}/avatars/${data.avatarUrl}` : null
                    });
                    setIsAuthenticated(true);
                }
            } catch (error) {
                console.error('Auth check failed:', error);
            }
        };
        checkAuth();
    }, []);

    const handleLogout = async () => {
        try {
            await fetch('http://localhost:5124/identity/User/Logout', {
                method: 'POST',
                credentials: 'include'
            });

            setIsAuthenticated(false);
            setUserData(null);
            navigate('/login');
        } catch (error) {
            console.error('Logout failed:', error);
        }
    };

    return (
        <header className={styles.header}>
            <div className={styles.logoContainer}>
                <img src={logo} alt="BN Logo" className={styles.logo} />
                <span className={styles.logoText}>BN</span>
            </div>

            <nav className={styles.nav}>
                <ul className={styles.navList}>
                    <li><Link to="/public" className={styles.navLink}>Главная</Link></li>
                    <li><Link to="/catalog" className={styles.navLink}>Каталог</Link></li>

                    {isAuthenticated && (
                        <>
                            <li><Link to="/bookmarks" className={styles.navLink}>Закладки</Link></li>
                            <li><Link to="/notifications" className={styles.navLink}>Уведомления</Link></li>
                        </>
                    )}

                    <li className={styles.profileMenu}>
                        <button
                            className={styles.menuButton}
                            onClick={() => setIsMenuOpen(!isMenuOpen)}
                        >
                            <img
                                src={userData?.avatarUrl || 'default-avatar.png'}
                                alt="Аватар"
                                className={styles.avatar}
                            />
                        </button>

                        {isMenuOpen && (
                            <div className={styles.dropdownMenu}>
                                <div className={styles.menuHeader}>
                                    <img
                                        src={userData?.avatarUrl || 'default-avatar.png'}
                                        alt="Аватар"
                                        className={styles.avatar}
                                    />
                                    <div>
                                        <p className={styles.profileName}>
                                            {userData?.userName || 'Гость'}
                                        </p>
                                        <p className={styles.profileEmail}>
                                            {userData?.email || 'Не авторизован'}
                                        </p>
                                    </div>
                                </div>

                                {isAuthenticated ? (
                                    <>
                                        <div className={styles.menuSection}>
                                            <Link to="/profile" className={styles.menuItem}>Профиль</Link>
                                            <Link to="/bookmarks" className={styles.menuItem}>Закладки</Link>
                                            <Link to="/notifications" className={styles.menuItem}>Уведомления</Link>
                                            <Link to="/catalog" className={styles.menuItem}>Каталог</Link>
                                        </div>

                                        <div className={styles.menuSection}>
                                            <Link to="/forum" className={styles.menuItem}>Форум</Link>
                                            <Link to="/about" className={styles.menuItem}>О нас</Link>
                                        </div>

                                        <div className={styles.menuSection}>
                                            <button
                                                className={styles.logoutButton}
                                                onClick={handleLogout}
                                            >
                                                Выйти
                                            </button>
                                        </div>
                                    </>
                                ) : (
                                    <div className={styles.menuSection}>
                                        <Link
                                            to="/register"
                                            className={styles.registerButton}
                                        >
                                            Зарегистрироваться
                                        </Link>
                                        <Link
                                            to="/login"
                                            className={styles.loginButton}
                                        >
                                            Войти
                                        </Link>
                                    </div>
                                )}
                            </div>
                        )}
                    </li>
                </ul>
            </nav>
        </header>
    );
};