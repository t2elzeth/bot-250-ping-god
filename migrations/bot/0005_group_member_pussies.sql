--liquibase formatted sql

--changeset uamangeldiev:10
create table bot.group_member_pussies
(
    id                  bigserial,
    group_member_id     bigint    not null,
    size                decimal   not null,
    last_grow_date_time timestamp not null,

    constraint pk_group_member_pussies primary key (id),

    constraint uq_group_member_id foreign key (group_member_id)
        references bot.group_members (id)
);
--rollback ;

--changeset uamangeldiev:20
alter table bot.group_member_pussies
    drop column group_member_id;
--rollback ;

--changeset uamangeldiev:30
alter table bot.group_members
    add column pussy_id bigint null,
    add constraint "uq_group_members#pussy" foreign key (pussy_id)
        references bot.group_member_pussies (id);
--rollback ;