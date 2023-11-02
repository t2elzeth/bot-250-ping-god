--liquibase formatted sql

--changeset uamangeldiev:10
create table bot.plain_message_answers
(
    id           bigserial    not null,
    message_text varchar(255) not null,
    answer_text  varchar(255) not null,

    constraint pk_plain_message_answers primary key (id)
);
--rollback ;

--changeset uamangeldiev:30
alter table bot.plain_message_answers
    add column is_disabled bool not null default false,
    add column min_similarity decimal not null default 0.7;

alter table bot.plain_message_answers
    alter column is_disabled drop default,
    alter column min_similarity drop default;
--rollback ;