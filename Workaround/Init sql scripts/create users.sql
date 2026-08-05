--create database "auth-users";

drop table if exists public.users;

create table if not exists public.users(
  id               bigint                     not null primary key generated always as identity,
  nickname         text                       not null unique,
  email            text                       not null unique, -- mb encrypted with salt
  password_hash    text                       not null,        -- + salt
  created_at       timestamp with time zone   not null
  -- timezone OR created_at with timezone
  -- language/country
  -- image url OR byte[]
);



drop table if exists public.roles;

create table if not exists public.roles(
  id            bigint                     not null primary key generated always as identity,
  name          text                       not null unique,
  created_at    timestamp with time zone   not null
);



drop table if exists public.user_roles;

create table if not exists public.user_roles(
  user_id       bigint                     not null,
  role_id       bigint                     not null,
  created_at    timestamp with time zone   not null,
  expires_at    timestamp with time zone,
  
  primary key (user_id, role_id)
);

create index if not exists ix_user_roles_expires_at on public.user_roles (expires_at);
