import st from '../assets/2151040272.jpg'

export interface Story {
    id: number;
    title: string;
    author: string;
    coverUrl: string;
    rating: number;
    views: number;
    tags: string[];
    isNew?: boolean;
}

export const mockStories: Story[] = [
    {
        id: 1,
        title: 'Путь демона',
        author: 'Иван Иванов',
        coverUrl: st,
        rating: 4.8,
        views: 12500,
        tags: ['Фэнтези', 'Приключения', '18+'],
        isNew: true
    },
    {
        id: 2,
        title: 'Меч бессмертия',
        author: 'Анна Петрова',
        coverUrl: st,
        rating: 4.5,
        views: 8900,
        tags: ['Боевик', 'Мистика']
    },
    {
        id: 3,
        title: 'Хроники Арканы',
        author: 'Сергей Смирнов',
        coverUrl: st,
        rating: 4.9,
        views: 23450,
        tags: ['Магия', 'Драма'],
        isNew: true
    },
    {
        id: 4,
        title: 'Тень империи',
        author: 'Мария Иванова',
        coverUrl: st,
        rating: 4.3,
        views: 6700,
        tags: ['Политика', 'История']
    },
    {
        id: 5,
        title: 'Звездный скиталец',
        author: 'Алексей Космосов',
        coverUrl: st,
        rating: 4.7,
        views: 14200,
        tags: ['Космос', 'Приключения']
    },
    {
        id: 6,
        title: 'Тайна черного леса',
        author: 'Ольга Темная',
        coverUrl: st,
        rating: 4.6,
        views: 9800,
        tags: ['Ужасы', 'Мистика'],
        isNew: true
    }
];