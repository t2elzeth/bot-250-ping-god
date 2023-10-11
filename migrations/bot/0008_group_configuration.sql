--liquibase formatted sql

--changeset uamangeldiev:10
alter table bot.groups
    add column allow_grow_pussy_command bool    not null default true,
    add column grow_pussy_min_size      decimal not null default -10,
    add column grow_pussy_max_size      decimal not null default 10;

alter table bot.groups
    alter column allow_grow_pussy_command drop default,
    alter column grow_pussy_min_size drop default,
    alter column grow_pussy_max_size drop default;
--rollback ;