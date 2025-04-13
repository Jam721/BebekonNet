// src/components/ProfilePage.tsx
import { useEffect, useState, useRef } from 'react';
import { Link, useNavigate } from "react-router-dom";
import ReactCrop, { type Crop } from 'react-image-crop';
import 'react-image-crop/dist/ReactCrop.css';
import styles from './ProfilePage.module.css';

interface UserData {
    userName: string;
    email: string;
    avatarUrl: string;
    createdAt: string;
}

export const ProfilePage = () => {
    useNavigate();
    const [userData, setUserData] = useState<UserData | null>(null);
    const [formData, setFormData] = useState({
        userName: '',
        email: '',
        password: '',
    });
    const [originalImage, setOriginalImage] = useState<string | null>(null);
    const [croppedImage, setCroppedImage] = useState<File | null>(null);
    const [crop, setCrop] = useState<Crop>({ unit: '%', width: 50, height: 50, x: 0, y: 0 });
    const [isCropModalOpen, setIsCropModalOpen] = useState(false);
    const imgRef = useRef<HTMLImageElement>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');
    const MINIO_BASE_URL = "http://localhost:9000";

    // Загрузка данных пользователя
    useEffect(() => {
        const fetchUserData = async () => {
            try {
                const response = await fetch('http://localhost:5124/identity/User/Me', {
                    credentials: 'include'
                });

                if (response.ok) {
                    const data = await response.json();
                    const user = {
                        ...data,
                        avatarUrl: data.avatarUrl ? `${MINIO_BASE_URL}/bebekonnet/${data.avatarUrl}` : null
                    };
                    setUserData(user);
                    setFormData({
                        userName: user.userName,
                        email: user.email,
                        password: ''
                    });
                }
            } catch (error) {
                console.error('Failed to fetch user data:', error);
            }
        };
        fetchUserData();
    }, []);

    // Обработчик выбора файла
    const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file) return;

        if (!file.type.startsWith('image/')) {
            setError('Пожалуйста, выберите изображение');
            return;
        }

        if (file.size > 2 * 1024 * 1024) {
            setError('Размер файла не должен превышать 2MB');
            return;
        }

        const reader = new FileReader();
        reader.onload = () => {
            setOriginalImage(reader.result as string);
            setIsCropModalOpen(true);
        };
        reader.readAsDataURL(file);
    };

    // Обрезка изображения
    const getCroppedImg = async (): Promise<File> => {
        try {
            const image = imgRef.current;
            if (!image || !originalImage) throw new Error('Изображение не выбрано');

            const canvas = document.createElement('canvas');
            const scaleX = image.naturalWidth / image.width;
            const scaleY = image.naturalHeight / image.height;

            canvas.width = crop.width * scaleX;
            canvas.height = crop.height * scaleY;

            const ctx = canvas.getContext('2d');
            if (!ctx) throw new Error('Ошибка инициализации canvas');

            ctx.drawImage(
                image,
                crop.x * scaleX,
                crop.y * scaleY,
                crop.width * scaleX,
                crop.height * scaleY,
                0,
                0,
                canvas.width,
                canvas.height
            );

            return new Promise((resolve, reject) => {
                canvas.toBlob((blob) => {
                    if (!blob) {
                        reject(new Error('Ошибка создания изображения'));
                        return;
                    }
                    resolve(new File([blob], 'avatar.jpg', { type: 'image/jpeg' }));
                }, 'image/jpeg', 0.9);
            });
        } catch (error) {
            throw new Error('Ошибка обрезки: ' + (error instanceof Error ? error.message : 'Unknown error'));
        }
    };

    // Подтверждение обрезки
    const handleCropComplete = async () => {
        try {
            const file = await getCroppedImg();
            setCroppedImage(file);
            setIsCropModalOpen(false);
            setError('');
        } catch (error) {
            setError(error instanceof Error ? error.message : 'Ошибка при обрезке');
        }
    };

    // Отправка формы
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setSuccess('');

        const form = new FormData();
        if (formData.userName) form.append('UserName', formData.userName);
        if (formData.email) form.append('Email', formData.email);
        if (formData.password) form.append('Password', formData.password);
        if (croppedImage) form.append('AvatarFile', croppedImage);

        try {
            const response = await fetch('http://localhost:5124/identity/User/UpdateUser', {
                method: 'PUT',
                credentials: 'include',
                body: form,
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Update failed');
            }

            const updatedUser = await response.json();
            setUserData(prev => ({
                ...prev!,
                ...updatedUser,
                avatarUrl: updatedUser.avatarUrl ?
                    `${MINIO_BASE_URL}/bebekonnet/${updatedUser.avatarUrl}` :
                    prev!.avatarUrl
            }));
            setCroppedImage(null);
            setSuccess('Профиль успешно обновлен!');
            setTimeout(() => setSuccess(''), 3000);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Ошибка обновления профиля');
        }
        window.location.href = '/';
    };

    if (!userData) return <div>Загрузка...</div>;

    return (
        <div className={styles.profileContainer}>
            <h2 className={styles.profileTitle}>Настройки профиля</h2>

            <form onSubmit={handleSubmit} className={styles.profileForm}>
                <div className={styles.avatarSection}>
                    <div
                        className={styles.avatarPreview}
                        onClick={() => fileInputRef.current?.click()}
                    >
                        {croppedImage ? (
                            <img
                                src={URL.createObjectURL(croppedImage)}
                                alt="Аватар"
                                className={styles.previewImage}
                            />
                        ) : (
                            <img
                                src={userData.avatarUrl || 'default-avatar.png'}
                                alt="Аватар"
                                className={styles.previewImage}
                            />
                        )}
                    </div>
                    <input
                        type="file"
                        ref={fileInputRef}
                        onChange={handleFileChange}
                        accept="image/*"
                        hidden
                    />
                </div>

                {/* Модальное окно обрезки */}
                {isCropModalOpen && originalImage && (
                    <div className={styles.modalOverlay}>
                        <div className={styles.cropModal}>
                            <h3>Обрежьте изображение</h3>
                            <div className={styles.cropContent}>
                                <div className={styles.cropImageContainer}>
                                    <ReactCrop
                                        crop={crop}
                                        onChange={c => setCrop(c)}
                                        aspect={1}
                                        minWidth={100}
                                        minHeight={100}
                                    >
                                        <img
                                            ref={imgRef}
                                            src={originalImage}
                                            alt="Обрезка"
                                            className={styles.cropImage}
                                        />
                                    </ReactCrop>
                                </div>
                            </div>
                            <div className={styles.modalButtons}>
                                <button
                                    type="button"
                                    onClick={() => setIsCropModalOpen(false)}
                                    className={styles.cancelButton}
                                >
                                    Отмена
                                </button>
                                <button
                                    type="button"
                                    onClick={handleCropComplete}
                                    className={styles.confirmButton}
                                >
                                    Сохранить
                                </button>
                            </div>
                        </div>
                    </div>
                )}

                <div className={styles.formGroup}>
                    <label>Имя пользователя</label>
                    <input
                        type="text"
                        value={formData.userName}
                        onChange={(e) => setFormData({...formData, userName: e.target.value})}
                    />
                </div>

                <div className={styles.formGroup}>
                    <label>Email</label>
                    <input
                        type="email"
                        value={formData.email}
                        onChange={(e) => setFormData({...formData, email: e.target.value})}
                    />
                </div>

                <div className={styles.formGroup}>
                    <label>Новый пароль</label>
                    <input
                        type="password"
                        value={formData.password}
                        onChange={(e) => setFormData({...formData, password: e.target.value})}
                        placeholder="Оставьте пустым чтобы сохранить текущий"
                    />
                </div>

                {error && <div className={styles.errorMessage}>{error}</div>}
                {success && <div className={styles.successMessage}>{success}</div>}

                <div className={styles.buttonGroup}>
                    <button type="submit" className={styles.saveButton}>
                        Сохранить изменения
                    </button>
                    <Link to="/" className={styles.cancelButton}>
                        Отмена
                    </Link>
                </div>
            </form>
        </div>
    );
};