--liquibase formatted sql

--changeset uamangeldiev:10
create table bot.group_members
(
    id               bigserial,
    username         varchar(255),
    anabruhate_count bigint not null
);
--rollback ;

--changeset uamangeldiev:20
alter table bot.group_members
    add column is_deleted bool not null default false;

alter table bot.group_members
    alter column is_deleted drop default;
--rollback ;