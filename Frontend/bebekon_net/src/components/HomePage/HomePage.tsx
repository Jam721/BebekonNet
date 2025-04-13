// src/components/HomePage.tsx
import { Link } from 'react-router-dom';
import { useRef } from 'react';
import styles from './HomePage.module.css';
import {mockStories, Story} from "../../mocks/stories.ts";


const StoryCard = ({ story }: { story: Story }) => (
    <div className={styles.storyCard}>
        <Link to={`/story/${story.id}`} className={styles.storyLink}>
            <div className={styles.coverContainer}>
                <img
                    src={story.coverUrl}
                    alt={story.title}
                    className={styles.coverImage}
                />
                {story.isNew && <div className={styles.newBadge}>Новое</div>}
            </div>
            <h3 className={styles.storyTitle}>{story.title}</h3>
            <p className={styles.storyAuthor}>{story.author}</p>
            <div className={styles.stats}>
                <span>★ {story.rating}</span>
                <span>👁 {story.views.toLocaleString()}</span>
            </div>
            <div className={styles.tags}>
                {story.tags.map(tag => (
                    <span key={tag} className={styles.tag}>{tag}</span>
                ))}
            </div>
        </Link>
    </div>
);

const StoryList = ({ stories, title }: { stories: Story[]; title: string }) => {
    const scrollRef = useRef<HTMLDivElement>(null);
    const scrollStep = 300; // Шаг прокрутки в пикселях

    const handleScroll = (direction: 'left' | 'right') => {
        if (scrollRef.current) {
            const scrollAmount = direction === 'right' ? scrollStep : -scrollStep;
            scrollRef.current.scrollBy({
                left: scrollAmount,
                behavior: 'smooth'
            });
        }
    };

    return (
        <section className={styles.section}>
            <br/>
            <div className={styles.sectionHeader}>
                <h2 className={styles.sectionTitle}>{title}</h2>
                <div className={styles.scrollControls}>
                    <button
                        onClick={() => handleScroll('left')}
                        className={styles.scrollButton}
                        aria-label="Прокрутить влево"
                    >
                        &lt;
                    </button>
                    <button
                        onClick={() => handleScroll('right')}
                        className={styles.scrollButton}
                        aria-label="Прокрутить вправо"
                    >
                        &gt;
                    </button>
                </div>
            </div>
            <div className={styles.storiesGrid} ref={scrollRef}>
                {stories.map(story => (
                    <StoryCard key={story.id} story={story} />
                ))}
            </div>
        </section>
    );
};

// src/components/HomePage.tsx
export const HomePage = () => {
    const popularStories = [...mockStories].sort((a, b) => b.views - a.views);
    const newStories = [...mockStories].filter(story => story.isNew);

    return (
        <div className={styles.container}>
            <StoryList title="Популярные сейчас" stories={popularStories} />
            <StoryList title="Новые" stories={newStories} />
        </div>
    );
};