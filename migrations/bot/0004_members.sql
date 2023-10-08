--liquibase formatted sql

--changeset uamangeldiev:10
create table bot.members
(
    id         bigserial    not null,
    username   varchar(255) null,
    first_name varchar(255) not null,
    last_name  varchar(255) null,
    user_id    bigint       not null,

    constraint pk_members primary key (id),

    constraint uq_username_user_id unique (username, user_id)
);
--rollback ;

--changeset uamangeldiev:30
alter table bot.group_members
    add column member_id bigint null,
    add constraint "fk_group_member#member" foreign key (member_id)
        references bot.members (id);
--rollback ;