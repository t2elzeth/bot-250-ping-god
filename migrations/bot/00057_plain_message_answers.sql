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