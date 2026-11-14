-- ============================================================================
-- COMMENTS-API DATABASE — Комментарии
-- Исправлено: дочерние комментарии привязаны к родительским из той же темы
-- ============================================================================

TRUNCATE TABLE comments RESTART IDENTITY;

-- Шаг 1: Создаём временную таблицу с шаблоном комментариев
-- Каждая строка: original_id, topic_id (1-100), is_parent (true/false)
DROP TABLE IF EXISTS temp_comment_template;
CREATE TEMP TABLE temp_comment_template AS
SELECT 
    generate_series as original_id,
    (1 + ((generate_series - 1 + (random() * 5)::int) % 100))::int as topic_id,
    CASE WHEN random() < 0.5 THEN false ELSE true END as is_parent
FROM generate_series(1, 10000);

-- Шаг 2: Вставляем родительские комментарии (is_parent = true)
-- Шаг 3: Вставляем дочерние комментарии (is_parent = false) с привязкой к родителям из той же темы
WITH parents_inserted AS (
    -- Вставляем всех родителей и возвращаем их ID
    INSERT INTO comments (topic_id, parent_comment_id, author_id, content, created_at, updated_at, updated_by_user_id, is_deleted, deleted_at)
    SELECT 
        topic_id,
        NULL,
        1 + (random() * 49)::int,
        (ARRAY[
            'Отличная статья! Очень полезно.',
            'Согласен, но есть нюансы.',
            'А можно подробнее об этом?',
            'Не совсем согласен, вот почему...',
            'Спасибо за обзор!',
            'Полезное руководство, спасибо!',
            'А что насчёт альтернативных подходов?',
            'Интересная точка зрения.',
            'Может быть стоит добавить пример кода?',
            'Прочитал документацию, автор прав.',
            'Не работает на реальном проекте, проверено на себе.',
            'Кто-нибудь пробовал на новой версии?',
            'Обязательно сохраню себе.',
            'Ждал эту тему очень давно!',
            'Отличный первый комментарий!',
            'А какие есть результаты тестов?',
            'Использую это на проекте уже год — работает стабильно.',
            'Нужно ещё учесть производительность.',
            'Хорошо расписано, но не хватает деталей.',
            'Спасибо за подробное объяснение!',
            'Можно ли это оптимизировать?',
            'Рекомендую также посмотреть связанные материалы.',
            'В моём случае работает иначе.',
            'А как быть с обратной совместимостью?',
            'Качественная проработка темы.',
            'Поднимаю эту тему снова.',
            'Спасибо, что объяснили для начинающих!',
            'А что думают опытные разработчики?',
            'Минус за отсутствие примеров.',
            'Плюс за структурированность!'
        ])[1 + (random() * 29)::int],
        NOW() - (random() * INTERVAL '30 days'),
        NOW() - (random() * INTERVAL '29 days'),
        CASE WHEN random() < 0.2 THEN 1 + (random() * 49)::int ELSE NULL END,
        CASE WHEN random() < 0.03 THEN true ELSE false END,
        CASE WHEN random() < 0.03 THEN NOW() - INTERVAL '1 day' ELSE NULL END
    FROM temp_comment_template
    WHERE is_parent = true
    RETURNING id, topic_id
),
parent_ids AS (
    -- Группируем ID родителей по темам для быстрого выбора
    SELECT 
        topic_id,
        array_agg(id ORDER BY id) as ids
    FROM parents_inserted
    GROUP BY topic_id
)
INSERT INTO comments (topic_id, parent_comment_id, author_id, content, created_at, updated_at, updated_by_user_id, is_deleted, deleted_at)
SELECT 
    child.topic_id,
    -- Выбираем случайного родителя из той же темы
    (SELECT ids[1 + floor(random() * array_length(ids, 1))::int] FROM parent_ids WHERE topic_id = child.topic_id),
    1 + (random() * 49)::int,
    (ARRAY[
        'Отличная статья! Очень полезно.',
        'Согласен, но есть нюансы.',
        'А можно подробнее об этом?',
        'Не совсем согласен, вот почему...',
        'Спасибо за обзор!',
        'Полезное руководство, спасибо!',
        'А что насчёт альтернативных подходов?',
        'Интересная точка зрения.',
        'Может быть стоит добавить пример кода?',
        'Прочитал документацию, автор прав.',
        'Не работает на реальном проекте, проверено на себе.',
        'Кто-нибудь пробовал на новой версии?',
        'Обязательно сохраню себе.',
        'Ждал эту тему очень давно!',
        'Отличный первый комментарий!',
        'А какие есть результаты тестов?',
        'Использую это на проекте уже год — работает стабильно.',
        'Нужно ещё учесть производительность.',
        'Хорошо расписано, но не хватает деталей.',
        'Спасибо за подробное объяснение!',
        'Можно ли это оптимизировать?',
        'Рекомендую также посмотреть связанные материалы.',
        'В моём случае работает иначе.',
        'А как быть с обратной совместимостью?',
        'Качественная проработка темы.',
        'Поднимаю эту тему снова.',
        'Спасибо, что объяснили для начинающих!',
        'А что думают опытные разработчики?',
        'Минус за отсутствие примеров.',
        'Плюс за структурированность!'
    ])[1 + (random() * 29)::int],
    NOW() - (random() * INTERVAL '30 days'),
    NOW() - (random() * INTERVAL '29 days'),
    CASE WHEN random() < 0.2 THEN 1 + (random() * 49)::int ELSE NULL END,
    CASE WHEN random() < 0.03 THEN true ELSE false END,
    CASE WHEN random() < 0.03 THEN NOW() - INTERVAL '1 day' ELSE NULL END
FROM temp_comment_template child
WHERE child.is_parent = false;

-- Итоговая статистика
SELECT COUNT(*) AS comments_created FROM comments;

-- Проверка: все ли дочерние комментарии привязаны к родителям из той же темы
SELECT 
    COUNT(*) AS mismatched_comments
FROM comments c
JOIN comments parent ON c.parent_comment_id = parent.id
WHERE c.topic_id != parent.topic_id;

-- Очистка временной таблицы
DROP TABLE IF EXISTS temp_comment_template;
