-- ============================================================================
-- AUTH-USERS DATABASE — Пользователи и роли
-- ============================================================================

TRUNCATE TABLE refresh_tokens RESTART IDENTITY;
TRUNCATE TABLE user_roles RESTART IDENTITY;
TRUNCATE TABLE users RESTART IDENTITY;

DO $$
DECLARE
    v_user_role   BIGINT;
    v_admin_role  BIGINT;
    v_mod_role    BIGINT;
BEGIN
    SELECT id INTO v_user_role   FROM roles WHERE name = 'User';
    SELECT id INTO v_admin_role  FROM roles WHERE name = 'Admin';
    SELECT id INTO v_mod_role    FROM roles WHERE name = 'Moderator';

    IF v_user_role IS NULL OR v_admin_role IS NULL OR v_mod_role IS NULL THEN
        RAISE EXCEPTION 'Не найдены необходимые роли!';
    END IF;

    -- Генерация пользователей (50 шт), ID = 1..50
    INSERT INTO users (nickname, email, password_hash, created_at)
    SELECT
        'user_' || i,
        'user_' || i || '@forum.example.com',
        md5('TestPassword123!'),
        NOW() - (random() * INTERVAL '30 days')
    FROM generate_series(1, 50) AS s(i);

    -- Пользователи 1-3: Admin + User
    INSERT INTO user_roles (user_id, role_id, created_at)
    SELECT u.id, v_admin_role, NOW() - INTERVAL '29 days'
    FROM users u WHERE u.id BETWEEN 1 AND 3;

    INSERT INTO user_roles (user_id, role_id, created_at)
    SELECT u.id, v_user_role, NOW() - INTERVAL '29 days'
    FROM users u WHERE u.id BETWEEN 1 AND 3;

    -- Пользователи 4-6: Moderator + User
    INSERT INTO user_roles (user_id, role_id, created_at)
    SELECT u.id, v_mod_role, NOW() - INTERVAL '28 days'
    FROM users u WHERE u.id BETWEEN 4 AND 6;

    INSERT INTO user_roles (user_id, role_id, created_at)
    SELECT u.id, v_user_role, NOW() - INTERVAL '28 days'
    FROM users u WHERE u.id BETWEEN 4 AND 6;

    -- Остальные (7-50): только User
    INSERT INTO user_roles (user_id, role_id, created_at)
    SELECT u.id, v_user_role, NOW() - INTERVAL '27 days'
    FROM users u WHERE u.id >= 7;

    RAISE NOTICE 'AuthUsersService: создано 50 пользователей, ID 1-50';
END $$;
