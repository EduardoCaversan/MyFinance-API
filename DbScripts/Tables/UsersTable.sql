-- Users table creation
CREATE TABLE dbo.users (
    id char(8) COLLATE Latin1_General_CS_AS NOT NULL DEFAULT ([dbo].[new_id]()),
    name nvarchar(200) NOT NULL,
    email nvarchar(200) NOT NULL UNIQUE,
    mobile_number varchar(15),
    creation_date datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    last_modified datetimeoffset,
    birthday_date datetimeoffset,
    is_sys_admin bit NOT NULL DEFAULT (0),
    password varchar(300) NOT NULL,
    ask_new_password bit NOT NULL DEFAULT (0),
    removed bit NOT NULL DEFAULT (0),
    CONSTRAINT PK_users PRIMARY KEY (id)
);

-- Unique index on column 'email'
CREATE UNIQUE INDEX UQ_users_email
ON dbo.users(email);