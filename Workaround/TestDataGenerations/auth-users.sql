-- ============================================================================
-- AUTH-USERS DATABASE — Пользователи и роли
-- ============================================================================
-- Пароли хранятся в виде BCrypt-хэшей.
-- Формулы паролей:
--   admin_N:     admin_Nadmin_N
--   moderator_N: moderator_Nmoderator_N
--   user_N:      user_Nuser_N
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

    -- Admin пользователи (ID 1-3): пароль = admin_Nadmin_N
    INSERT INTO users (nickname, email, password_hash, created_at) VALUES
        ('admin_1', 'admin_1@forum.example.com', '$2a$12$Q698PD/aKGR8GTyIGoNrOeV10I5ArkLa4GsJkUma/viWjKW.lTMei', NOW() - (random() * INTERVAL '30 days')),
        ('admin_2', 'admin_2@forum.example.com', '$2a$12$Q698PD/aKGR8GTyIGoNrOeJ7kLBbcnGylAMxwkXJQbZp12tVPqCtq', NOW() - (random() * INTERVAL '30 days')),
        ('admin_3', 'admin_3@forum.example.com', '$2a$12$Q698PD/aKGR8GTyIGoNrOeAFn5eEesGq88pIA7RH8DPl20rVnMqf2', NOW() - (random() * INTERVAL '30 days'));

    -- Moderator пользователи (ID 4-6): пароль = moderator_Nmoderator_N
    INSERT INTO users (nickname, email, password_hash, created_at) VALUES
        ('moderator_1', 'moderator_1@forum.example.com', '$2a$12$8rrZzSyNfJj9j0w271QIS.MNvWfGfBkdE9Fbz4AeSlzEN50TONIQW', NOW() - (random() * INTERVAL '30 days')),
        ('moderator_2', 'moderator_2@forum.example.com', '$2a$12$8rrZzSyNfJj9j0w271QIS.oXN1UANntAyNVVRb6ZHS5YBG9dyKC1C', NOW() - (random() * INTERVAL '30 days')),
        ('moderator_3', 'moderator_3@forum.example.com', '$2a$12$8rrZzSyNfJj9j0w271QIS.Z7BFtU8wRj.pUSsAUTLrNkYG0UdvASu', NOW() - (random() * INTERVAL '30 days'));

    -- User пользователи (ID 7-50): пароль = user_Nuser_N
    INSERT INTO users (nickname, email, password_hash, created_at) VALUES
        ('user_1',  'user_1@forum.example.com',  '$2a$12$iPlA1rrZf6ScigfR0cKinuNKXo50RA0AXyztp6Zcp9EcnUWSBKom.',  NOW() - (random() * INTERVAL '30 days')),
        ('user_2',  'user_2@forum.example.com',  '$2a$12$iPlA1rrZf6ScigfR0cKinu3Vft8HbDCWkoj/gp9PqEDwv6abW.RMu',  NOW() - (random() * INTERVAL '30 days')),
        ('user_3',  'user_3@forum.example.com',  '$2a$12$iPlA1rrZf6ScigfR0cKinuu6pZbdGs0y9J7Cl.6szBp2/guILdAse',  NOW() - (random() * INTERVAL '30 days')),
        ('user_4',  'user_4@forum.example.com',  '$2a$12$iPlA1rrZf6ScigfR0cKinu55oqGEM/EwTgDqGg00qPSqeNitnggB6',  NOW() - (random() * INTERVAL '30 days')),
        ('user_5',  'user_5@forum.example.com',  '$2a$12$iPlA1rrZf6ScigfR0cKinu8X2HU4s3nISFNcTJV74y/.vc2hJDP7y',  NOW() - (random() * INTERVAL '30 days')),
        ('user_6',  'user_6@forum.example.com',  '$2a$12$iPlA1rrZf6ScigfR0cKinugiJvEiRAmLqQLVLQulRFCPpWWd6nw2a',  NOW() - (random() * INTERVAL '30 days')),
        ('user_7',  'user_7@forum.example.com',  '$2a$12$iPlA1rrZf6ScigfR0cKinuZAuqE9isWza.Gsd2VwogzXasQKQeefG',  NOW() - (random() * INTERVAL '30 days')),
        ('user_8',  'user_8@forum.example.com',  '$2a$12$iPlA1rrZf6ScigfR0cKinucc6D9eeRztg6W3LHu.THVfMUXea1os2',  NOW() - (random() * INTERVAL '30 days')),
        ('user_9',  'user_9@forum.example.com',  '$2a$12$iPlA1rrZf6ScigfR0cKinuL0mzsKLp0337KasCTGhaz8KY1SpibCi',  NOW() - (random() * INTERVAL '30 days')),
        ('user_10', 'user_10@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuWWyujOswhWxYRuej7qOBSUOYOn9ITuS',  NOW() - (random() * INTERVAL '30 days')),
        ('user_11', 'user_11@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuoxbkPhxiCCCDE/aBB797okjpYI0.rIi',  NOW() - (random() * INTERVAL '30 days')),
        ('user_12', 'user_12@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinukUi.GWl0.LbfNCXUsDgZywKoMs3QU4O',  NOW() - (random() * INTERVAL '30 days')),
        ('user_13', 'user_13@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinugj8CI2RBQnPKH77Q5mMiXVdefouWazO',  NOW() - (random() * INTERVAL '30 days')),
        ('user_14', 'user_14@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuc/QxIN0HmgqhLnJYoynPumi34nf3jwO',  NOW() - (random() * INTERVAL '30 days')),
        ('user_15', 'user_15@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuyiRC9kWUy1wkrdiZBe0Sj8xyjfsNCYO',  NOW() - (random() * INTERVAL '30 days')),
        ('user_16', 'user_16@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinu2UPwfv1SsKqIRlNCjTJEjoUVtj1xrKi',  NOW() - (random() * INTERVAL '30 days')),
        ('user_17', 'user_17@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinu2xWDH1ZfFESevAN6UG7lS.h7NbFpcbG',  NOW() - (random() * INTERVAL '30 days')),
        ('user_18', 'user_18@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuiZi93/90I2Msu4Zz66lsIDtvYK9f1kq',  NOW() - (random() * INTERVAL '30 days')),
        ('user_19', 'user_19@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuYfCL4WzT1opf6wUYuQNa0arW6yWS7zG',  NOW() - (random() * INTERVAL '30 days')),
        ('user_20', 'user_20@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuyN16UOQHRJnmZcjYLPPE7J8.DKZAjw6',  NOW() - (random() * INTERVAL '30 days')),
        ('user_21', 'user_21@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuKbcvOpErgEAxFbbKXDoAxiCFjLcnveK',  NOW() - (random() * INTERVAL '30 days')),
        ('user_22', 'user_22@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuvWvOeyjjPTIrEPHL.0lGEJz8.y/2RXS',  NOW() - (random() * INTERVAL '30 days')),
        ('user_23', 'user_23@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuQuY0MWRm2Iwn078xUhjLnBYAfS3kCO.',  NOW() - (random() * INTERVAL '30 days')),
        ('user_24', 'user_24@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuQO.flw0inV1bQIIgoe9xNrXUbO0Qwgy',  NOW() - (random() * INTERVAL '30 days')),
        ('user_25', 'user_25@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuYiSw03PEQrvj5maywUJMIef0pYWo6Ti',  NOW() - (random() * INTERVAL '30 days')),
        ('user_26', 'user_26@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinucn/e6Wla/8X0S1aXNL0QoJSOj0a9uQu',  NOW() - (random() * INTERVAL '30 days')),
        ('user_27', 'user_27@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuOguUyHww7asEeXTtwmVR9y4xYiiyVhO',  NOW() - (random() * INTERVAL '30 days')),
        ('user_28', 'user_28@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuhHw9yEeCMvb9cVYzmsJzdohTUDwToNa',  NOW() - (random() * INTERVAL '30 days')),
        ('user_29', 'user_29@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuR8BPDiHVphGFhOADlbOGb7sfnrjMYcu',  NOW() - (random() * INTERVAL '30 days')),
        ('user_30', 'user_30@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuCz97O7lxOoTZyl/hrXSq8nBZYd/2tdS',  NOW() - (random() * INTERVAL '30 days')),
        ('user_31', 'user_31@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuLRSFS5toBCNN9UOiefzs7oMxdWOa/IC',  NOW() - (random() * INTERVAL '30 days')),
        ('user_32', 'user_32@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuabvYdpRQJmY/0LbeZieIl980SuFzg0m',  NOW() - (random() * INTERVAL '30 days')),
        ('user_33', 'user_33@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuH9YL8mvy46ILP8Bq.eB8vpd7g.W.X0C',  NOW() - (random() * INTERVAL '30 days')),
        ('user_34', 'user_34@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinufzWy6fHPlAmwWDX/L7GENopF3okPvdq',  NOW() - (random() * INTERVAL '30 days')),
        ('user_35', 'user_35@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinu.5khRZcG08pc5ort2mtzhbXyEb63fQC',  NOW() - (random() * INTERVAL '30 days')),
        ('user_36', 'user_36@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuO7pL1TTtv9aiS.eyqHLY4yzP5UXLtAG',  NOW() - (random() * INTERVAL '30 days')),
        ('user_37', 'user_37@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuf9GED4MxJLthJgiATGmXZ9/Flda6ghu',  NOW() - (random() * INTERVAL '30 days')),
        ('user_38', 'user_38@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuwyrOdJI0CQ6KNhBVfO5XlxsIvf0sZ6e',  NOW() - (random() * INTERVAL '30 days')),
        ('user_39', 'user_39@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuiOtvysyobM8wwIx.i0X7mzc9woOLwha',  NOW() - (random() * INTERVAL '30 days')),
        ('user_40', 'user_40@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinu7L.HMh0yR4hDbSsDqMh16GUqDjj4CFi',  NOW() - (random() * INTERVAL '30 days')),
        ('user_41', 'user_41@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuqChD.WkIkBwDsCghjYAYVU5ksZrMqz2',  NOW() - (random() * INTERVAL '30 days')),
        ('user_42', 'user_42@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinud/Ep8VX2Pk8Z6nqc9GK7k8gxRaLmz0O',  NOW() - (random() * INTERVAL '30 days')),
        ('user_43', 'user_43@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinurwUjZaY4/eKNfTdfw3GClB0Jp1EAfr2',  NOW() - (random() * INTERVAL '30 days')),
        ('user_44', 'user_44@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuNQSwvtPldj7p7oZcjCdfRrXIqT6ELyK',  NOW() - (random() * INTERVAL '30 days')),
        ('user_45', 'user_45@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinu5xZq/KK4owUyqyEOCAsLjsItfyTmR3.',  NOW() - (random() * INTERVAL '30 days')),
        ('user_46', 'user_46@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinu0ZoAsSffEtKppUydBH/u1kGz90ehX7e',  NOW() - (random() * INTERVAL '30 days')),
        ('user_47', 'user_47@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuo4JYCloe/gtlZa3AWpnauaCmDCI4R0q',  NOW() - (random() * INTERVAL '30 days')),
        ('user_48', 'user_48@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinuzj93.PMAwJiTb5HI0mvJDW5beeNrEPC',  NOW() - (random() * INTERVAL '30 days')),
        ('user_49', 'user_49@forum.example.com', '$2a$12$iPlA1rrZf6ScigfR0cKinu2n8Kmmm9P1Cy1lw2GghWVFANSZNh1Ka',  NOW() - (random() * INTERVAL '30 days'));

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
