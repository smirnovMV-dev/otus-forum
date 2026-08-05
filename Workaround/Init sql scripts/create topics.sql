--create database "topics-api";

drop table if exists public.topics;

create table if not exists public.topics(
  id               bigint                     not null primary key generated always as identity,
  title            text                       not null,
  author_id        bigint                     not null,
  created_at       timestamp with time zone   not null
);
