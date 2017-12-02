USE [master]
GO
/****** Object:  Database [Payment]    Script Date: 11/30/2017 15:40:36 ******/
CREATE DATABASE [Payment] ON  PRIMARY 
( NAME = N'Payment', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\Payment.mdf' , SIZE = 2304KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Payment_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\Payment_log.LDF' , SIZE = 576KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Payment] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Payment].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Payment] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [Payment] SET ANSI_NULLS OFF
GO
ALTER DATABASE [Payment] SET ANSI_PADDING OFF
GO
ALTER DATABASE [Payment] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [Payment] SET ARITHABORT OFF
GO
ALTER DATABASE [Payment] SET AUTO_CLOSE ON
GO
ALTER DATABASE [Payment] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [Payment] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [Payment] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [Payment] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [Payment] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [Payment] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [Payment] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [Payment] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [Payment] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [Payment] SET  ENABLE_BROKER
GO
ALTER DATABASE [Payment] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [Payment] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [Payment] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [Payment] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [Payment] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [Payment] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [Payment] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [Payment] SET  READ_WRITE
GO
ALTER DATABASE [Payment] SET RECOVERY SIMPLE
GO
ALTER DATABASE [Payment] SET  MULTI_USER
GO
ALTER DATABASE [Payment] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [Payment] SET DB_CHAINING OFF
GO
USE [Payment]
GO
/****** Object:  Table [dbo].[Username]    Script Date: 11/30/2017 15:40:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Username](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](50) NOT NULL,
	[password] [nvarchar](50) NOT NULL,
	[ctime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[InsertLogin]    Script Date: 11/30/2017 15:40:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertLogin] @username nvarchar(30), @password nvarchar(50)
AS
BEGIN
declare @count int
set @count= (SELECT COUNT(*) FROM dbo.Username where username=@username and
                                                    password=@password)
IF @count = 0                                                  
BEGIN
INSERT INTO dbo.Username(username, password, ctime)VALUES (@username, @password, GETDATE())
END
END
GO
/****** Object:  Table [dbo].[Counts]    Script Date: 11/30/2017 15:40:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Counts](
	[name] [nvarchar](30) NOT NULL,
	[ctime] [datetime] NULL,
	[idUser] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[showLogin]    Script Date: 11/30/2017 15:40:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[showLogin] @username nvarchar(30), @password nvarchar(30),  @ID int output
as


BEGIN
 SELECT  * FROM dbo.Username WHERE 
          username = @username AND
          password =@password
END

Begin
      SELECT  @ID= id FROM dbo.Username WHERE 
          username = @username AND
          password =@password
End

RETURN @ID
GO
/****** Object:  Table [dbo].[Payments]    Script Date: 11/30/2017 15:40:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[number] [bigint] IDENTITY(1,1) NOT NULL,
	[NameCounts] [nvarchar](30) NOT NULL,
	[name] [nvarchar](30) NOT NULL,
	[suma] [decimal](18, 2) NOT NULL,
	[datum] [datetime] NOT NULL,
	[description] [nvarchar](30) NULL,
PRIMARY KEY CLUSTERED 
(
	[number] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[ShowNameCounts]    Script Date: 11/30/2017 15:40:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Script for SelectTopNRows command from SSMS  ******/
CREATE PROCEDURE [dbo].[ShowNameCounts] @idUser int
as
begin
SELECT name as Counts FROM dbo.Counts 
WHERE idUser = @idUser
GROUP BY name
end
GO
/****** Object:  StoredProcedure [dbo].[totalSum]    Script Date: 11/30/2017 15:40:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[totalSum] 
 @from datetime, @to datetime, @name nvarchar(30), @CountName nvarchar(30), @idUser int 
as
begin
if @name = '' AND @CountName <> ''
begin
SELECT c.name AS Counts, p.name ,SUM(suma) 'price', COUNT(number) as number FROM dbo.Counts c
inner join dbo.Payments p on c.name=p.NameCounts
where  datum >= @from AND
       datum <= @to   AND
       c.name = @CountName AND
       c.idUser = @idUser
GROUP BY c.name, p.name
ORDER BY number ASC
end


ELSE IF @name <> '' AND @CountName =''
begin
SELECT c.name AS Counts,p.name, SUM(suma) 'price', COUNT(number) as number FROM dbo.Counts c
inner join dbo.Payments p on c.name=p.NameCounts
where  datum >= @from AND
       datum <= @to   AND
       p.name = @name AND
       c.idUser = @idUser
GROUP BY c.name, p.name
ORDER BY number ASC
end

ELSE IF @name = '' AND @CountName =''
begin
SELECT c.name AS Counts,p.name, SUM(suma) 'price', COUNT(number) as number FROM dbo.Counts c
inner join dbo.Payments p on c.name=p.NameCounts
where  datum >= @from AND
       datum <= @to   AND
       c.idUser = idUser    
GROUP BY c.name, p.name
ORDER BY number ASC
end


else
begin
SELECT c.name as Counts,p.name, SUM(suma) 'price', COUNT(number) as number FROM dbo.Counts c
inner join dbo.Payments p on c.name=p.NameCounts
where p.name = @name   AND
      datum >= @from AND
      datum <= @to   AND
      c.name = @CountName AND
      c.idUser = @idUser
GROUP BY c.name, p.name
ORDER BY number ASC
end
end

--select * from dbo.Payments
GO
/****** Object:  StoredProcedure [dbo].[ShowPayments]    Script Date: 11/30/2017 15:40:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ShowPayments] 
@FROM DATETIME, @TO DATETIME, @name nvarchar(30), @CountName nvarchar(30), @idUser int

as
-- IF I HAVE TWO EMPTY STRING 
begin
  if @name ='' AND @CountName =''
   begin
     select  c.name as Counts, p.number 'serial', p.name, p.suma, p.datum, description  from [Payment].[dbo].[Payments] p
     inner join dbo.Counts c on p.NameCounts = c.name
WHERE datum >= @FROM AND 
      datum <= @TO   AND
      c.idUser = @idUser
      
GROUP BY c.name,p.number,  p.name, p.suma, p.datum, p.description
   end
   
-- IF I HAVE @NAME EMPTY STRING AND NAME COUNTS   
else if @name = '' AND @CountName != ''
     begin
    select c.name as Counts, p.number 'serial', p.name, p.suma, p.datum, description from [Payment].[dbo].Counts c
    inner join dbo.Payments p on c.name = p.NameCounts
WHERE datum >= @FROM AND 
      datum <= @TO   AND   
      c.name = @CountName AND
      c.idUser = @idUser
GROUP BY c.name, p.number, p.name, p.suma, p.datum, p.description
      end
      
-- IF I HAVE @COUNT NAME EMPTY AND STRING  @NAME
else if @name <> '' AND @CountName = ''
     begin
    select c.name as Counts, p.number 'serial', p.name, p.suma, p.datum, description from [Payment].[dbo].Counts c
    inner join dbo.Payments p on c.name = p.NameCounts
WHERE datum >= @FROM AND 
      datum <= @TO   AND   
      p.name=@name   AND
      c.idUser =@idUser
GROUP BY c.name, p.number, p.name, p.suma, p.datum, p.description
      end      
      
      
-- TWO  STRING NOT EMPTY      
else
       begin
    select c.name as Counts, p.number 'serial', p.name, p.suma, p.datum, description from [Payment].[dbo].Counts c
    inner join dbo.Payments p on c.name=p.NameCounts
WHERE datum >= @FROM AND 
      datum <= @TO   AND
      c.name =@CountName    AND   
      p.name = @name AND
      c.idUser=@idUser
GROUP BY c.name, p.number, p.name, p.suma, p.datum, p.description
      end
end
GO
/****** Object:  StoredProcedure [dbo].[Showname]    Script Date: 11/30/2017 15:40:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Showname] @idUser int
as
begin
SELECT p.name  FROM dbo.Payments p INNER JOIN dbo.Counts c ON p.NameCounts = c.name 
WHERE c.idUser = @idUser
GROUP BY p.name
end
GO
/****** Object:  StoredProcedure [dbo].[DeletePayments]    Script Date: 11/30/2017 15:40:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeletePayments] @name nvarchar(20), @NameCounts nvarchar(30), @id int
as

begin 
delete   from  dbo.Payments  
where name = @name AND
      number=@id AND
      NameCounts=@NameCounts
      
if @NameCounts not in (select NameCounts from dbo.Payments) 
   begin
      delete from dbo.Counts
      where name=@NameCounts
   end  
end
GO
/****** Object:  StoredProcedure [dbo].[InsertPayments]    Script Date: 11/30/2017 15:40:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertPayments] 
--insert parametars
@idUser int,
@NameCount nvarchar(30),
@name nvarchar(30), 
@suma decimal(18,2),
@datum datetime,
@description nvarchar(30)
AS

BEGIN
IF @NameCount NOT IN(SELECT name FROM dbo.Counts)
begin
INSERT INTO dbo.Counts(name,ctime, idUser)VALUES(@NameCount, GETDATE(),@idUser)
end
INSERT INTO dbo.Payments(NameCounts,name,suma,datum,description)VALUES(@NameCount,@name,@suma,@datum,@description)
END
GO
/****** Object:  Check [CK__Payments__suma__440B1D61]    Script Date: 11/30/2017 15:40:48 ******/
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD CHECK  (([SUMA]>(0)))
GO
/****** Object:  Check [CK__Payments__suma__44FF419A]    Script Date: 11/30/2017 15:40:48 ******/
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD CHECK  (([SUMA]>=(0)))
GO
/****** Object:  ForeignKey [FK__Counts__idUser__48CFD27E]    Script Date: 11/30/2017 15:40:48 ******/
ALTER TABLE [dbo].[Counts]  WITH CHECK ADD FOREIGN KEY([idUser])
REFERENCES [dbo].[Username] ([id])
GO
/****** Object:  ForeignKey [FK_Count]    Script Date: 11/30/2017 15:40:48 ******/
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Count] FOREIGN KEY([NameCounts])
REFERENCES [dbo].[Counts] ([name])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Count]
GO
