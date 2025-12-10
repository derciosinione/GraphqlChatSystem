create database R2yChatSystem;

go

use R2yChatSystem;

go

create table Users
(
    Email varchar(40) primary key,
    Name  varchar(100) not null
);

go

CREATE TABLE ChatRooms
(
    Id           UNIQUEIDENTIFIER PRIMARY KEY,
    Type         INT           NOT NULL, -- enum RoomType
    Name         NVARCHAR(255) NULL,
    Participants NVARCHAR(MAX) NULL,     -- JSON array: ChatRoomParticipant[]
    Messages     NVARCHAR(MAX) NULL,     -- JSON array: Message[] with nested Poll, Votes, Reply, ReadBy
    CreatedAt    DATETIME2     NOT NULL
);


select *
from Users;

go 

INSERT INTO Users (Email, Name) VALUES
                                    ('ana.silva@example.com', 'Ana Silva'),
                                    ('bruno.costa@example.com', 'Bruno Costa'),
                                    ('maria.santos@example.com', 'Maria Santos'),
                                    ('joao.pereira@example.com', 'João Pereira'),
                                    ('sofia.ferreira@example.com', 'Sofia Ferreira'),
                                    ('ricardo.monteiro@example.com', 'Ricardo Monteiro'),
                                    ('ines.rocha@example.com', 'Inês Rocha'),
                                    ('carlos.gomes@example.com', 'Carlos Gomes'),
                                    ('luisa.marques@example.com', 'Luísa Marques'),
                                    ('miguel.ramos@example.com', 'Miguel Ramos'),
                                    ('patricia.matos@example.com', 'Patrícia Matos'),
                                    ('diogo.lopes@example.com', 'Diogo Lopes'),
                                    ('beatriz.faria@example.com', 'Beatriz Faria'),
                                    ('paulo.machado@example.com', 'Paulo Machado'),
                                    ('lara.coelho@example.com', 'Lara Coelho'),
                                    ('rui.teixeira@example.com', 'Rui Teixeira'),
                                    ('carla.pinto@example.com', 'Carla Pinto'),
                                    ('tiago.moraes@example.com', 'Tiago Moraes'),
                                    ('valeria.torres@example.com', 'Valéria Torres'),
                                    ('david.neves@example.com', 'David Neves'),
                                    ('raquel.borges@example.com', 'Raquel Borges'),
                                    ('andre.melo@example.com', 'André Melo'),
                                    ('vera.almeida@example.com', 'Vera Almeida'),
                                    ('fabio.sousa@example.com', 'Fábio Sousa'),
                                    ('sara.dias@example.com', 'Sara Dias'),
                                    ('nuno.figueiredo@example.com', 'Nuno Figueiredo'),
                                    ('helena.vieira@example.com', 'Helena Vieira'),
                                    ('ze.luis@example.com', 'Zé Luís'),
                                    ('vasco.moura@example.com', 'Vasco Moura'),
                                    ('marta.palmeira@example.com', 'Marta Palmeira');