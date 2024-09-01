-- Run this scripts on master (You may need SA or other admin level user to this)
-- Database creation (ON MASTER)
CREATE DATABASE myfinance
GO
-- Login creation (ON MASTER)
CREATE LOGIN application
	WITH PASSWORD = 'a!4f8e?2b7c3d9f1e0@2'
GO

-- API user basic configuration
CREATE USER application FROM LOGIN application
WITH DEFAULT_SCHEMA = [dbo];
GO
EXEC sp_addrolemember 'db_ddladmin', 'application';
GO
EXEC sp_addrolemember 'db_datawriter', 'application';
GO
EXEC sp_addrolemember 'db_datareader', 'application';
GO
GRANT REFERENCES TO application;  
GO
GRANT EXECUTE TO application; --- RUN dbo.new_id
GO
GRANT VIEW DATABASE STATE TO application; --- RUN rebuild_indexs
GO
ALTER SERVER ROLE ##MS_ServerStateManager## ADD MEMBER application; --- RUN DBCC DROPCLEANBUFFERS
GO
GRANT SHOWPLAN TO application;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- View for rand values
CREATE VIEW [dbo].[rand_view]
AS
    SELECT RAND() VALUE;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Function to randomize values
CREATE FUNCTION [dbo].[get_random](@min INT, @max INT) RETURNS INT AS
BEGIN
    RETURN ROUND(((@max-@min-1)*(SELECT r.value
    FROM rand_view r)+@min),0);
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Function to get random characters
CREATE FUNCTION [dbo].[get_random_chars](@chars VARCHAR(max), @len INT) RETURNS VARCHAR(max) AS
BEGIN
    DECLARE @aux varchar(max)
    set @aux='';
    DECLARE @count int
    set @count=0;
    WHILE (@count<@len)
	BEGIN
        DECLARE @r INT
        SET @r=dbo.get_random(1,len(@chars));
        SET @aux+=substring(@chars,@r,1);
        SET @count+=1;
    END
    RETURN @aux;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Function to get random 8 character ids
CREATE FUNCTION [dbo].[new_id]() returns char(8) as
BEGIN
    return [dbo].[get_random_chars]('0123456789ABCDEFGHIJKLMNOPQRSTUVXWYZabcdefghijklmnopqrstuvxwyz',(8));
END
GO