import { useState, useRef } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import ReactCrop, { type Crop } from 'react-image-crop';
import 'react-image-crop/dist/ReactCrop.css';
import styles from './Register.module.css';

export const Register = () => {
    const navigate = useNavigate();
    const [username, setUsername] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [originalImage, setOriginalImage] = useState<string | null>(null);
    const [croppedImage, setCroppedImage] = useState<File | null>(null);
    const [crop, setCrop] = useState<Crop>({
        unit: '%',
        width: 50,
        height: 50,
        x: 0,
        y: 0 // Добавлены обязательные координаты
    });
    const [isCropModalOpen, setIsCropModalOpen] = useState(false);
    const imgRef = useRef<HTMLImageElement>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    // Обработчик выбора файла
    const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file) return;

        if (!file.type.startsWith('image/')) {
            alert('Пожалуйста, выберите изображение');
            return;
        }

        if (file.size > 2 * 1024 * 1024) {
            alert('Размер файла не должен превышать 2MB');
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
        } catch (error) {
            throw new Error('Ошибка при обрезке изображения'+(error instanceof Error ? error.message : 'Unknown error'));
        }
    };

    // Отправка формы
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!croppedImage) {
            alert('Пожалуйста, выберите и обрежьте аватар');
            return;
        }

        const formData = new FormData();
        formData.append('UserName', username);
        formData.append('Email', email);
        formData.append('Password', password);
        formData.append('AvatarFile', croppedImage);

        try {
            const response = await fetch('http://localhost:5124/identity/User/Register', {
                method: 'POST',
                body: formData,
            });

            if (response.ok) navigate('/login');
            else alert('Ошибка регистрации');
        } catch (error) {
            throw new Error('Ошибка при обрезке изображения'+(error instanceof Error ? error.message : 'Unknown error'));
        }
    };

    return (
        <div className={styles.container}>
            <form className={styles.form} onSubmit={handleSubmit}>
                <h2 className={styles.title}>Регистрация</h2>

                {/* Поле для загрузки аватарки */}
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
                            <span className={styles.uploadText}>
                                Нажмите для загрузки аватарки
                            </span>
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
                                            style={{
                                                maxWidth: '100%',
                                                maxHeight: '100vh'
                                            }}
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

                <div className={styles.inputGroup}>
                    <label className={styles.label}>Имя пользователя</label>
                    <input
                        type="text"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        className={styles.input}
                        required
                    />
                </div>

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

                <button type="submit" className={styles.submitButton}>
                    Зарегистрироваться
                </button>

                <p className={styles.linkText}>
                    Уже есть аккаунт?{' '}
                    <Link to="/login" className={styles.link}>
                        Войти
                    </Link>
                </p>
            </form>
        </div>
    );
};