--create database "comments-api";

drop table if exists public.comments;

create table if not exists public.comments(
  id                  bigint                     not null primary key generated always as identity,
  topic_id            bigint                     not null,
  parent_comment_id   bigint                     null,
  author_id           bigint                     not null,
  content             text                       not null,
  created_at          timestamp with time zone   not null,
  updated_at          timestamp with time zone   not null
);

create index if not exists ix_comments_topic_id on public.comments (topic_id);