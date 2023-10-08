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

--changeset uamangeldiev:30
alter table bot.group_members
    add constraint pk_group_members primary key (id);
--rollback ;

--changeset uamangeldiev:40
alter table bot.group_members
    add column chat_id bigint null;
--rollback ;

--changeset uamangeldiev:50
alter table bot.group_members
    add column last_anabruhate_date_time  timestamp null,
    add column last_hour_anabruhate_count bigint    not null default 0;
--rollback ;

--changeset uamangeldiev:60
alter table bot.group_members
    alter column last_anabruhate_date_time set not null;
--rollback ;