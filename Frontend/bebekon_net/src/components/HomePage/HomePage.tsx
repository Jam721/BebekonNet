// src/components/HomePage.tsx
import { Link } from 'react-router-dom';
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

const StoryList = ({ stories, title }: { stories: Story[]; title: string }) => (
    <section className={styles.section}>
        <h2 className={styles.sectionTitle}>{title}</h2>
        <div className={styles.storiesGrid}>
            {stories.map(story => (
                <StoryCard key={story.id} story={story} />
            ))}
        </div>
    </section>
);

export const HomePage = () => {
    const popularStories = [...mockStories].sort((a, b) => b.views - a.views);
    const newStories = [...mockStories].filter(story => story.isNew);

    return (
        <div className={styles.container}>
            <StoryList title="Популярные сейчас" stories={popularStories} />
            <StoryList title="Новые поступления" stories={newStories} />
        </div>
    );
};