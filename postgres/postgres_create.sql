CREATE DATABASE "suavemusicstore";

\connect "suavemusicstore"

DROP TABLE IF EXISTS "genres" CASCADE;

CREATE TABLE "genres"(
    "genreid" SERIAL PRIMARY KEY NOT NULL,
    "name" varchar(120) NULL,
    "description" varchar(4000) NULL);

INSERT INTO "genres" ("genreid", "name", "description") VALUES (1, 'Rock', 'Rock and Roll is a form of rock music developed in the 1950s and 1960s. Rock music combines many kinds of music from "the" United States, such as Country music, folk music, church music, work songs, blues and jazz.');
INSERT INTO "genres" ("genreid", "name", "description") VALUES (2, 'Jazz', 'Jazz is a type of music which was invented in the United States. Jazz music combines African-American music with European music. Some common jazz instruments include the saxoPhone, trumpet, piano, double bass, and drums.');
INSERT INTO "genres" ("genreid", "name", "description") VALUES (3, 'Metal', 'Heavy Metal is a loud, aggressive style of Rock music. The bands who play heavy-metal music usually have one or two guitars, a bass guitar and drums. In some bands, electronic keyboards, organs, or other instruments are used. Heavy metal songs are loud and powerful-sounding, and have strong rhythms that are repeated. There are many different types of Heavy Metal, some of which are described below. Heavy metal bands sometimes dress in jeans, leather jackets, and leather boots, and have long hair. Heavy metal bands sometimes behave in a dramatic way when they play their instruments or sing. However, many heavy metal bands do not like to do this.');
INSERT INTO "genres" ("genreid", "name", "description") VALUES (4, 'Alternative', 'Alternative rock is a type of rock music that became popular in the 1980s and became widely popular in the 1990s. Alternative rock is made up of various subgenres that have come out of the indie music scene since the 1980s, such as grunge, indie rock, Britpop, gothic rock, and indie pop. These genres are sorted by their collective types of punk, which laid the groundwork for alternative music in the 1970s.');
INSERT INTO "genres" ("genreid", "name", "description") VALUES (5, 'Disco', 'Disco is a style of pop music that was popular in the mid-1970s. Disco music has a strong beat that people can dance to. People usually dance to disco music at bars called disco clubs. The word "disco" is also used to refer to the style of dancing that people do to disco music, or to the style of clothes that people wear to go disco dancing. Disco was at its most popular in the United States and Europe in the 1970s and early 1980s. Disco was brought into the mainstream by the hit movie Saturday Night Fever, which was released in 1977. This movie, which starred John Travolta, showed people doing disco dancing. Many radio stations played disco in the late 1970s.');
INSERT INTO "genres" ("genreid", "name", "description") VALUES (6, 'Blues', 'The blues is a form of music that started in the United States during the start of the 20th century. It was started by former African slaves from "spirituals", praise songs, and chants. The first blues songs were called Delta blues. These songs came from "the" area near the mouth of the Mississippi River.');
INSERT INTO "genres" ("genreid", "name", "description") VALUES (7, 'Latin', 'Latin American music is the music of all Countries in Latin America (and the Caribbean) and comes in many varieties. Latin America is home to musical styles such as the simple, rural conjunto music of northern Mexico, the sophisticated habanera of Cuba, the rhythmic sounds of the Puerto Rican plena, the symphonies of Heitor Villa-Lobos, and the simple and moving Andean flute. Music has played an important part recently in Latin America''s politics, the nueva canción movement being a prime example. Latin music is very diverse, with the only truly unifying thread being the use of Latin-derived languages, predominantly the Spanish language, the Portuguese language in Brazil, and to a lesser extent, Latin-derived creole languages, such as those found in Haiti.');
INSERT INTO "genres" ("genreid", "name", "description") VALUES (8, 'Reggae', 'Reggae is a music genre first developed in Jamaica in the late 1960s. While sometimes used in a broader sense to refer to most types of Jamaican music, the term reggae more properly denotes a particular music style that originated following on the development of ska and rocksteady.');
INSERT INTO "genres" ("genreid", "name", "description") VALUES (9, 'Pop', 'Pop music is a music genre that developed from "the" mid-1950s as a softer alternative to rock ''n'' roll and later to rock music. It has a focus on commercial recording, often oriented towards a youth market, usually through the medium of relatively short and simple love songs. While these basic elements of the genre have remained fairly constant, pop music has absorbed influences from "most" other forms of popular music, particularly borrowing from "the" development of rock music, and utilizing key technological innovations to produce new variations on existing themes.');
INSERT INTO "genres" ("genreid", "name", "description") VALUES (10, 'Classical', 'Classical music is a very general term which normally refers to the standard music of Countries in the Western world. It is music that has been composed by musicians who are trained in the art of writing music (composing) and written down in music notation so that other musicians can play it. Classical music can also be described as "art music" because great art (skill) is needed to compose it and to perform it well. Classical music differs from "pop" music because it is not made just in order to be popular for a short time or just to be a commercial success.');

DROP TABLE IF EXISTS "orders";

CREATE TABLE "orders"(
	"orderid" SERIAL PRIMARY KEY NOT NULL,
	"orderdate" timestamptz NOT NULL,
	"username" varchar(256) NULL,
	"firstname" varchar(160) NULL,
	"lastname" varchar(160) NULL,
	"address" varchar(70) NULL,
	"city" varchar(40) NULL,
	"state" varchar(40) NULL,
	"postalcode" varchar(10) NULL,
	"country" varchar(40) NULL,
	"phone" varchar(24) NULL,
	"email" varchar(160) NULL,
	"total" numeric(10, 2) NOT NULL);

DROP TABLE IF EXISTS "artists";

CREATE TABLE "artists"(
	"artistid" SERIAL PRIMARY KEY NOT NULL,
	"name" varchar(120) NULL);

INSERT INTO "artists" ("artistid", "name") VALUES (1, 'AC/DC');
INSERT INTO "artists" ("artistid", "name") VALUES (2, 'Accept');
INSERT INTO "artists" ("artistid", "name") VALUES (3, 'Aerosmith');
INSERT INTO "artists" ("artistid", "name") VALUES (4, 'Alanis Morissette');
INSERT INTO "artists" ("artistid", "name") VALUES (5, 'Alice In Chains');
INSERT INTO "artists" ("artistid", "name") VALUES (6, 'Antônio Carlos Jobim');
INSERT INTO "artists" ("artistid", "name") VALUES (7, 'Apocalyptica');
INSERT INTO "artists" ("artistid", "name") VALUES (8, 'Audioslave');
INSERT INTO "artists" ("artistid", "name") VALUES (10, 'Billy Cobham');
INSERT INTO "artists" ("artistid", "name") VALUES (11, 'Black Label Society');
INSERT INTO "artists" ("artistid", "name") VALUES (12, 'Black Sabbath');
INSERT INTO "artists" ("artistid", "name") VALUES (14, 'Bruce Dickinson');
INSERT INTO "artists" ("artistid", "name") VALUES (15, 'Buddy Guy');
INSERT INTO "artists" ("artistid", "name") VALUES (16, 'Caetano Veloso');
INSERT INTO "artists" ("artistid", "name") VALUES (17, 'Chico Buarque');
INSERT INTO "artists" ("artistid", "name") VALUES (18, 'Chico Science & Nação Zumbi');
INSERT INTO "artists" ("artistid", "name") VALUES (19, 'Cidade Negra');
INSERT INTO "artists" ("artistid", "name") VALUES (20, 'Cláudio Zoli');
INSERT INTO "artists" ("artistid", "name") VALUES (21, 'Various Artists');
INSERT INTO "artists" ("artistid", "name") VALUES (22, 'Led Zeppelin');
INSERT INTO "artists" ("artistid", "name") VALUES (23, 'Frank Zappa & Captain Beefheart');
INSERT INTO "artists" ("artistid", "name") VALUES (24, 'Marcos Valle');
INSERT INTO "artists" ("artistid", "name") VALUES (27, 'Gilberto Gil');
INSERT INTO "artists" ("artistid", "name") VALUES (37, 'Ed Motta');
INSERT INTO "artists" ("artistid", "name") VALUES (41, 'Elis Regina');
INSERT INTO "artists" ("artistid", "name") VALUES (42, 'Milton Nascimento');
INSERT INTO "artists" ("artistid", "name") VALUES (46, 'Jorge Ben');
INSERT INTO "artists" ("artistid", "name") VALUES (50, 'Metallica');
INSERT INTO "artists" ("artistid", "name") VALUES (51, 'Queen');
INSERT INTO "artists" ("artistid", "name") VALUES (52, 'Kiss');
INSERT INTO "artists" ("artistid", "name") VALUES (53, 'Spyro Gyra');
INSERT INTO "artists" ("artistid", "name") VALUES (55, 'David Coverdale');
INSERT INTO "artists" ("artistid", "name") VALUES (56, 'Gonzaguinha');
INSERT INTO "artists" ("artistid", "name") VALUES (58, 'Deep Purple');
INSERT INTO "artists" ("artistid", "name") VALUES (59, 'Santana');
INSERT INTO "artists" ("artistid", "name") VALUES (68, 'Miles Davis');
INSERT INTO "artists" ("artistid", "name") VALUES (72, 'Vinícius De Moraes');
INSERT INTO "artists" ("artistid", "name") VALUES (76, 'Creedence Clearwater Revival');
INSERT INTO "artists" ("artistid", "name") VALUES (77, 'Cássia Eller');
INSERT INTO "artists" ("artistid", "name") VALUES (79, 'Dennis Chambers');
INSERT INTO "artists" ("artistid", "name") VALUES (80, 'Djavan');
INSERT INTO "artists" ("artistid", "name") VALUES (81, 'Eric Clapton');
INSERT INTO "artists" ("artistid", "name") VALUES (82, 'Faith No More');
INSERT INTO "artists" ("artistid", "name") VALUES (83, 'Falamansa');
INSERT INTO "artists" ("artistid", "name") VALUES (84, 'Foo Fighters');
INSERT INTO "artists" ("artistid", "name") VALUES (86, 'Funk Como Le Gusta');
INSERT INTO "artists" ("artistid", "name") VALUES (87, 'Godsmack');
INSERT INTO "artists" ("artistid", "name") VALUES (88, 'Guns N'' Roses');
INSERT INTO "artists" ("artistid", "name") VALUES (89, 'Incognito');
INSERT INTO "artists" ("artistid", "name") VALUES (90, 'Iron Maiden');
INSERT INTO "artists" ("artistid", "name") VALUES (92, 'Jamiroquai');
INSERT INTO "artists" ("artistid", "name") VALUES (94, 'Jimi Hendrix');
INSERT INTO "artists" ("artistid", "name") VALUES (95, 'Joe Satriani');
INSERT INTO "artists" ("artistid", "name") VALUES (96, 'Jota Quest');
INSERT INTO "artists" ("artistid", "name") VALUES (98, 'Judas Priest');
INSERT INTO "artists" ("artistid", "name") VALUES (99, 'Legião Urbana');
INSERT INTO "artists" ("artistid", "name") VALUES (100, 'Lenny Kravitz');
INSERT INTO "artists" ("artistid", "name") VALUES (101, 'Lulu Santos');
INSERT INTO "artists" ("artistid", "name") VALUES (102, 'Marillion');
INSERT INTO "artists" ("artistid", "name") VALUES (103, 'Marisa Monte');
INSERT INTO "artists" ("artistid", "name") VALUES (105, 'Men At Work');
INSERT INTO "artists" ("artistid", "name") VALUES (106, 'Motörhead');
INSERT INTO "artists" ("artistid", "name") VALUES (109, 'Mötley Crüe');
INSERT INTO "artists" ("artistid", "name") VALUES (110, 'Nirvana');
INSERT INTO "artists" ("artistid", "name") VALUES (111, 'O Terço');
INSERT INTO "artists" ("artistid", "name") VALUES (112, 'Olodum');
INSERT INTO "artists" ("artistid", "name") VALUES (113, 'Os Paralamas Do Sucesso');
INSERT INTO "artists" ("artistid", "name") VALUES (114, 'Ozzy Osbourne');
INSERT INTO "artists" ("artistid", "name") VALUES (115, 'Page & Plant');
INSERT INTO "artists" ("artistid", "name") VALUES (117, 'Paul D''Ianno');
INSERT INTO "artists" ("artistid", "name") VALUES (118, 'Pearl Jam');
INSERT INTO "artists" ("artistid", "name") VALUES (120, 'Pink Floyd');
INSERT INTO "artists" ("artistid", "name") VALUES (124, 'R.E.M.');
INSERT INTO "artists" ("artistid", "name") VALUES (126, 'Raul Seixas');
INSERT INTO "artists" ("artistid", "name") VALUES (127, 'Red Hot Chili Peppers');
INSERT INTO "artists" ("artistid", "name") VALUES (128, 'Rush');
INSERT INTO "artists" ("artistid", "name") VALUES (130, 'Skank');
INSERT INTO "artists" ("artistid", "name") VALUES (132, 'Soundgarden');
INSERT INTO "artists" ("artistid", "name") VALUES (133, 'Stevie Ray Vaughan & Double Trouble');
INSERT INTO "artists" ("artistid", "name") VALUES (134, 'Stone Temple Pilots');
INSERT INTO "artists" ("artistid", "name") VALUES (135, 'System Of A Down');
INSERT INTO "artists" ("artistid", "name") VALUES (136, 'Terry Bozzio, Tony Levin & Steve Stevens');
INSERT INTO "artists" ("artistid", "name") VALUES (137, 'The Black Crowes');
INSERT INTO "artists" ("artistid", "name") VALUES (139, 'The Cult');
INSERT INTO "artists" ("artistid", "name") VALUES (140, 'The Doors');
INSERT INTO "artists" ("artistid", "name") VALUES (141, 'The Police');
INSERT INTO "artists" ("artistid", "name") VALUES (142, 'The Rolling Stones');
INSERT INTO "artists" ("artistid", "name") VALUES (144, 'The Who');
INSERT INTO "artists" ("artistid", "name") VALUES (145, 'Tim Maia');
INSERT INTO "artists" ("artistid", "name") VALUES (150, 'U2');
INSERT INTO "artists" ("artistid", "name") VALUES (151, 'UB40');
INSERT INTO "artists" ("artistid", "name") VALUES (152, 'Van Halen');
INSERT INTO "artists" ("artistid", "name") VALUES (153, 'Velvet Revolver');
INSERT INTO "artists" ("artistid", "name") VALUES (155, 'Zeca Pagodinho');
INSERT INTO "artists" ("artistid", "name") VALUES (157, 'Dread Zeppelin');
INSERT INTO "artists" ("artistid", "name") VALUES (179, 'Scorpions');
INSERT INTO "artists" ("artistid", "name") VALUES (196, 'Cake');
INSERT INTO "artists" ("artistid", "name") VALUES (197, 'Aisha Duo');
INSERT INTO "artists" ("artistid", "name") VALUES (200, 'The Posies');
INSERT INTO "artists" ("artistid", "name") VALUES (201, 'Luciana Souza/Romero Lubambo');
INSERT INTO "artists" ("artistid", "name") VALUES (202, 'Aaron Goldberg');
INSERT INTO "artists" ("artistid", "name") VALUES (203, 'Nicolaus Esterhazy Sinfonia');
INSERT INTO "artists" ("artistid", "name") VALUES (204, 'Temple of the Dog');
INSERT INTO "artists" ("artistid", "name") VALUES (205, 'Chris Cornell');
INSERT INTO "artists" ("artistid", "name") VALUES (206, 'Alberto Turco & Nova Schola Gregoriana');
INSERT INTO "artists" ("artistid", "name") VALUES (208, 'English Concert & Trevor Pinnock');
INSERT INTO "artists" ("artistid", "name") VALUES (211, 'Wilhelm Kempff');
INSERT INTO "artists" ("artistid", "name") VALUES (212, 'Yo-Yo Ma');
INSERT INTO "artists" ("artistid", "name") VALUES (213, 'Scholars Baroque Ensemble');
INSERT INTO "artists" ("artistid", "name") VALUES (217, 'Royal Philharmonic Orchestra & Sir Thomas Beecham');
INSERT INTO "artists" ("artistid", "name") VALUES (219, 'Britten Sinfonia, Ivor Bolton & Lesley Garrett');
INSERT INTO "artists" ("artistid", "name") VALUES (221, 'Sir Georg Solti & Wiener Philharmoniker');
INSERT INTO "artists" ("artistid", "name") VALUES (223, 'London Symphony Orchestra & Sir Charles Mackerras');
INSERT INTO "artists" ("artistid", "name") VALUES (224, 'Barry Wordsworth & BBC Concert Orchestra');
INSERT INTO "artists" ("artistid", "name") VALUES (226, 'Eugene Ormandy');
INSERT INTO "artists" ("artistid", "name") VALUES (229, 'Boston Symphony Orchestra & Seiji Ozawa');
INSERT INTO "artists" ("artistid", "name") VALUES (230, 'Aaron Copland & London Symphony Orchestra');
INSERT INTO "artists" ("artistid", "name") VALUES (231, 'Ton Koopman');
INSERT INTO "artists" ("artistid", "name") VALUES (232, 'Sergei Prokofiev & Yuri Temirkanov');
INSERT INTO "artists" ("artistid", "name") VALUES (233, 'Chicago Symphony Orchestra & Fritz Reiner');
INSERT INTO "artists" ("artistid", "name") VALUES (234, 'Orchestra of The Age of Enlightenment');
INSERT INTO "artists" ("artistid", "name") VALUES (236, 'James Levine');
INSERT INTO "artists" ("artistid", "name") VALUES (237, 'Berliner Philharmoniker & Hans Rosbaud');
INSERT INTO "artists" ("artistid", "name") VALUES (238, 'Maurizio Pollini');
INSERT INTO "artists" ("artistid", "name") VALUES (240, 'Gustav Mahler');
INSERT INTO "artists" ("artistid", "name") VALUES (242, 'Edo de Waart & San Francisco Symphony');
INSERT INTO "artists" ("artistid", "name") VALUES (244, 'Choir Of Westminster Abbey & Simon Preston');
INSERT INTO "artists" ("artistid", "name") VALUES (245, 'Michael Tilson Thomas & San Francisco Symphony');
INSERT INTO "artists" ("artistid", "name") VALUES (247, 'The King''s Singers');
INSERT INTO "artists" ("artistid", "name") VALUES (248, 'Berliner Philharmoniker & Herbert Von Karajan');
INSERT INTO "artists" ("artistid", "name") VALUES (250, 'Christopher O''Riley');
INSERT INTO "artists" ("artistid", "name") VALUES (251, 'Fretwork');
INSERT INTO "artists" ("artistid", "name") VALUES (252, 'Amy Winehouse');
INSERT INTO "artists" ("artistid", "name") VALUES (253, 'Calexico');
INSERT INTO "artists" ("artistid", "name") VALUES (255, 'Yehudi Menuhin');
INSERT INTO "artists" ("artistid", "name") VALUES (258, 'Les Arts Florissants & William Christie');
INSERT INTO "artists" ("artistid", "name") VALUES (259, 'The 12 Cellists of The Berlin Philharmonic');
INSERT INTO "artists" ("artistid", "name") VALUES (260, 'Adrian Leaper & Doreen de Feis');
INSERT INTO "artists" ("artistid", "name") VALUES (261, 'Roger Norrington, London Classical Players');
INSERT INTO "artists" ("artistid", "name") VALUES (264, 'Kent Nagano and Orchestre de l''Opéra de Lyon');
INSERT INTO "artists" ("artistid", "name") VALUES (265, 'Julian Bream');
INSERT INTO "artists" ("artistid", "name") VALUES (266, 'Martin Roscoe');
INSERT INTO "artists" ("artistid", "name") VALUES (267, 'Göteborgs Symfoniker & Neeme Järvi');
INSERT INTO "artists" ("artistid", "name") VALUES (270, 'Gerald Moore');
INSERT INTO "artists" ("artistid", "name") VALUES (271, 'Mela Tenenbaum, Pro Musica Prague & Richard Kapp');
INSERT INTO "artists" ("artistid", "name") VALUES (274, 'Nash Ensemble');
INSERT INTO "artists" ("artistid", "name") VALUES (276, 'Chic');
INSERT INTO "artists" ("artistid", "name") VALUES (277, 'Anita Ward');
INSERT INTO "artists" ("artistid", "name") VALUES (278, 'Donna Summer');

DROP TABLE IF EXISTS "albums" CASCADE;

CREATE TABLE "albums"(
	"albumid" SERIAL PRIMARY KEY NOT NULL,
	"genreid" int NOT NULL,
	"artistid" int NOT NULL,
	"title" varchar(160) NOT NULL,
	"price" numeric(10, 2) NOT NULL,
	"albumarturl" varchar(1024) NULL CONSTRAINT DF_Album_AlbumArtUrl  DEFAULT ('/placeholder.gif'));
    
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (1, 1, 1, 'For Those About To Rock We Salute You', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (2, 1, 1, 'Let There Be Rock', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (3, 1, 100, 'Greatest Hits', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (4, 1, 102, 'Misplaced Childhood', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (5, 1, 105, 'The Best Of Men At Work', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (7, 1, 110, 'Nevermind', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (8, 1, 111, 'Compositores', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (9, 1, 114, 'Bark at the Moon (Remastered)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (10, 1, 114, 'Blizzard of Ozz', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (11, 1, 114, 'Diary of a Madman (Remastered)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (12, 1, 114, 'No More Tears (Remastered)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (13, 1, 114, 'Speak of the Devil', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (14, 1, 115, 'Walking Into Clarksdale', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (15, 1, 117, 'The Beast Live', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (16, 1, 118, 'Live On Two Legs [Live]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (17, 1, 118, 'Riot Act', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (18, 1, 118, 'Ten', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (19, 1, 118, 'Vs.', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (20, 1, 120, 'Dark Side Of The Moon', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (21, 1, 124, 'New Adventures In Hi-Fi', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (22, 1, 126, 'Raul Seixas', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (23, 1, 127, 'By The Way', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (24, 1, 127, 'Californication', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (25, 1, 128, 'Retrospective I (1974-1980)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (26, 1, 130, 'Maquinarama', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (27, 1, 130, 'O Samba Poconé', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (28, 1, 132, 'A-Sides', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (29, 1, 134, 'Core', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (30, 1, 136, '[1997] Black Light Syndrome', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (31, 1, 139, 'Beyond Good And Evil', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (33, 1, 140, 'The Doors', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (34, 1, 141, 'The Police Greatest Hits', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (35, 1, 142, 'Hot Rocks, 1964-1971 (Disc 1)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (36, 1, 142, 'No Security', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (37, 1, 142, 'Voodoo Lounge', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (38, 1, 144, 'My Generation - The Very Best Of The Who', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (39, 1, 150, 'Achtung Baby', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (40, 1, 150, 'B-Sides 1980-1990', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (41, 1, 150, 'How To Dismantle An Atomic Bomb', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (42, 1, 150, 'Pop', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (43, 1, 150, 'Rattle And Hum', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (44, 1, 150, 'The Best Of 1980-1990', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (45, 1, 150, 'War', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (46, 1, 150, 'Zooropa', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (47, 1, 152, 'Diver Down', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (48, 1, 152, 'The Best Of Van Halen, Vol. I', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (49, 1, 152, 'Van Halen III', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (50, 1, 152, 'Van Halen', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (51, 1, 153, 'Contraband', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (52, 1, 157, 'Un-Led-Ed', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (54, 1, 2, 'Balls to the Wall', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (55, 1, 2, 'Restless and Wild', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (56, 1, 200, 'Every Kind of Light', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (57, 1, 22, 'BBC Sessions [Disc 1] [Live]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (58, 1, 22, 'BBC Sessions [Disc 2] [Live]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (59, 1, 22, 'Coda', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (60, 1, 22, 'Houses Of The Holy', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (61, 1, 22, 'In Through The Out Door', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (62, 1, 22, 'IV', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (63, 1, 22, 'Led Zeppelin I', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (64, 1, 22, 'Led Zeppelin II', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (65, 1, 22, 'Led Zeppelin III', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (66, 1, 22, 'Physical Graffiti [Disc 1]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (67, 1, 22, 'Physical Graffiti [Disc 2]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (68, 1, 22, 'Presence', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (69, 1, 22, 'The Song Remains The Same (Disc 1)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (70, 1, 22, 'The Song Remains The Same (Disc 2)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (71, 1, 23, 'Bongo Fury', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (72, 1, 3, 'Big Ones', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (73, 1, 4, 'Jagged Little Pill', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (74, 1, 5, 'Facelift', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (75, 1, 51, 'Greatest Hits I', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (76, 1, 51, 'Greatest Hits II', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (77, 1, 51, 'News Of The World', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (78, 1, 52, 'Greatest Kiss', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (79, 1, 52, 'Unplugged [Live]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (80, 1, 55, 'Into The Light', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (81, 1, 58, 'Come Taste The Band', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (82, 1, 58, 'Deep Purple In Rock', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (83, 1, 58, 'Fireball', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (84, 1, 58, 'Machine Head', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (85, 1, 58, 'MK III The Final Concerts [Disc 1]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (86, 1, 58, 'Purpendicular', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (87, 1, 58, 'Slaves And Masters', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (88, 1, 58, 'Stormbringer', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (89, 1, 58, 'The Battle Rages On', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (90, 1, 58, 'The Final Concerts (Disc 2)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (91, 1, 59, 'Santana - As Years Go By', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (92, 1, 59, 'Santana Live', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (93, 1, 59, 'Supernatural', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (94, 1, 76, 'Chronicle, Vol. 1', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (95, 1, 76, 'Chronicle, Vol. 2', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (96, 1, 8, 'Audioslave', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (97, 1, 82, 'King For A Day Fool For A Lifetime', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (98, 1, 84, 'In Your Honor [Disc 1]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (99, 1, 84, 'In Your Honor [Disc 2]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (100, 1, 84, 'The Colour And The Shape', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (101, 1, 88, 'Appetite for Destruction', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (102, 1, 88, 'Use Your Illusion I', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (103, 1, 90, 'A Matter of Life and Death', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (104, 1, 90, 'Brave New World', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (105, 1, 90, 'Fear Of The Dark', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (106, 1, 90, 'Live At Donington 1992 (Disc 1)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (107, 1, 90, 'Live At Donington 1992 (Disc 2)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (108, 1, 90, 'Rock In Rio [CD2]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (109, 1, 90, 'The Number of The Beast', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (110, 1, 90, 'The X Factor', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (111, 1, 90, 'Virtual XI', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (112, 1, 92, 'Emergency On Planet Earth', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (113, 1, 94, 'Are You Experienced?', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (114, 1, 95, 'Surfing with the Alien (Remastered)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (115, 10, 203, 'The Best of Beethoven', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (119, 10, 208, 'Pachelbel: Canon & Gigue', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (122, 10, 211, 'Bach: Goldberg Variations', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (123, 10, 212, 'Bach: The Cello Suites', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (124, 10, 213, 'Handel: The Messiah (Highlights)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (128, 10, 217, 'Haydn: Symphonies 99 - 104', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (130, 10, 219, 'A Soprano Inspired', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (132, 10, 221, 'Wagner: Favourite Overtures', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (134, 10, 223, 'Tchaikovsky: The Nutcracker', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (135, 10, 224, 'The Last Night of the Proms', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (138, 10, 226, 'Respighi:Pines of Rome', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (139, 10, 226, 'Strauss: Waltzes', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (140, 10, 229, 'Carmina Burana', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (141, 10, 230, 'A Copland Celebration, Vol. I', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (142, 10, 231, 'Bach: Toccata & Fugue in D Minor', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (143, 10, 232, 'Prokofiev: Symphony No.1', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (144, 10, 233, 'Scheherazade', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (145, 10, 234, 'Bach: The Brandenburg Concertos', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (147, 10, 236, 'Mascagni: Cavalleria Rusticana', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (148, 10, 237, 'Sibelius: Finlandia', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (152, 10, 242, 'Adams, John: The Chairman Dances', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (154, 10, 245, 'Berlioz: Symphonie Fantastique', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (155, 10, 245, 'Prokofiev: Romeo & Juliet', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (157, 10, 247, 'English Renaissance', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (159, 10, 248, 'Mozart: Symphonies Nos. 40 & 41', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (161, 10, 250, 'SCRIABIN: Vers la flamme', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (163, 10, 255, 'Bartok: Violin & Viola Concertos', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (166, 10, 259, 'South American Getaway', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (167, 10, 260, 'Górecki: Symphony No. 3', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (168, 10, 261, 'Purcell: The Fairy Queen', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (171, 10, 264, 'Weill: The Seven Deadly Sins', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (173, 10, 266, 'Szymanowski: Piano Works, Vol. 1', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (174, 10, 267, 'Nielsen: The Six Symphonies', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (177, 10, 274, 'Mozart: Chamber Music', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (178, 2, 10, 'The Best Of Billy Cobham', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (179, 2, 197, 'Quiet Songs', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (180, 2, 202, 'Worlds', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (181, 2, 27, 'Quanta Gente Veio ver--Bônus De Carnaval', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (182, 2, 53, 'Heart of the Night', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (183, 2, 53, 'Morning Dance', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (184, 2, 6, 'Warner 25 Anos', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (185, 2, 68, 'Miles Ahead', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (186, 2, 68, 'The Essential Miles Davis [Disc 1]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (187, 2, 68, 'The Essential Miles Davis [Disc 2]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (188, 2, 79, 'Outbreak', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (189, 2, 89, 'Blue Moods', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (190, 3, 100, 'Greatest Hits', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (191, 3, 106, 'Ace Of Spades', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (192, 3, 109, 'Motley Crue Greatest Hits', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (193, 3, 11, 'Alcohol Fueled Brewtality Live! [Disc 1]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (194, 3, 11, 'Alcohol Fueled Brewtality Live! [Disc 2]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (195, 3, 114, 'Tribute', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (196, 3, 12, 'Black Sabbath Vol. 4 (Remaster)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (197, 3, 12, 'Black Sabbath', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (198, 3, 135, 'Mezmerize', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (199, 3, 14, 'Chemical Wedding', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (200, 3, 50, '...And Justice For All', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (201, 3, 50, 'Black Album', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (202, 3, 50, 'Garage Inc. (Disc 1)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (203, 3, 50, 'Garage Inc. (Disc 2)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (204, 3, 50, 'Load', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (205, 3, 50, 'Master Of Puppets', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (206, 3, 50, 'ReLoad', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (207, 3, 50, 'Ride The Lightning', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (208, 3, 50, 'St. Anger', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (209, 3, 7, 'Plays Metallica By Four Cellos', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (210, 3, 87, 'Faceless', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (211, 3, 88, 'Use Your Illusion II', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (212, 3, 90, 'A Real Dead One', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (213, 3, 90, 'A Real Live One', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (214, 3, 90, 'Live After Death', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (215, 3, 90, 'No Prayer For The Dying', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (216, 3, 90, 'Piece Of Mind', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (217, 3, 90, 'Powerslave', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (218, 3, 90, 'Rock In Rio [CD1]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (219, 3, 90, 'Rock In Rio [CD2]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (220, 3, 90, 'Seventh Son of a Seventh Son', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (221, 3, 90, 'Somewhere in Time', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (222, 3, 90, 'The Number of The Beast', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (223, 3, 98, 'Living After Midnight', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (224, 4, 196, 'Cake: B-Sides and Rarities', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (225, 4, 204, 'Temple of the Dog', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (226, 4, 205, 'Carry On', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (227, 4, 253, 'Carried to Dust (Bonus Track Version)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (228, 4, 8, 'Revelations', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (229, 6, 133, 'In Step', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (230, 6, 137, 'Live [Disc 1]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (231, 6, 137, 'Live [Disc 2]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (233, 6, 81, 'The Cream Of Clapton', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (234, 6, 81, 'Unplugged', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (235, 6, 90, 'Iron Maiden', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (238, 7, 103, 'Barulhinho Bom', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (239, 7, 112, 'Olodum', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (240, 7, 113, 'Acústico MTV', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (241, 7, 113, 'Arquivo II', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (242, 7, 113, 'Arquivo Os Paralamas Do Sucesso', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (243, 7, 145, 'Serie Sem Limite (Disc 1)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (244, 7, 145, 'Serie Sem Limite (Disc 2)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (245, 7, 155, 'Ao Vivo [IMPORT]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (246, 7, 16, 'Prenda Minha', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (247, 7, 16, 'Sozinho Remix Ao Vivo', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (248, 7, 17, 'Minha Historia', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (249, 7, 18, 'Afrociberdelia', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (250, 7, 18, 'Da Lama Ao Caos', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (251, 7, 20, 'Na Pista', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (252, 7, 201, 'Duos II', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (253, 7, 21, 'Sambas De Enredo 2001', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (254, 7, 21, 'Vozes do MPB', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (255, 7, 24, 'Chill: Brazil (Disc 1)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (256, 7, 27, 'Quanta Gente Veio Ver (Live)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (257, 7, 37, 'The Best of Ed Motta', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (258, 7, 41, 'Elis Regina-Minha História', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (259, 7, 42, 'Milton Nascimento Ao Vivo', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (260, 7, 42, 'Minas', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (261, 7, 46, 'Jorge Ben Jor 25 Anos', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (262, 7, 56, 'Meus Momentos', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (263, 7, 6, 'Chill: Brazil (Disc 2)', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (264, 7, 72, 'Vinicius De Moraes', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (266, 7, 77, 'Cássia Eller - Sem Limite [Disc 1]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (267, 7, 80, 'Djavan Ao Vivo - Vol. 02', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (268, 7, 80, 'Djavan Ao Vivo - Vol. 1', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (269, 7, 81, 'Unplugged', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (270, 7, 83, 'Deixa Entrar', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (271, 7, 86, 'Roda De Funk', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (272, 7, 96, 'Jota Quest-1995', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (274, 7, 99, 'Mais Do Mesmo', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (275, 8, 100, 'Greatest Hits', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (276, 8, 151, 'UB40 The Best Of - Volume Two [UK]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (277, 8, 19, 'Acústico MTV [Live]', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (278, 8, 19, 'Cidade Negra - Hits', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (280, 9, 21, 'Axé Bahia 2001', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (281, 9, 252, 'Frank', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (282, 5, 276, 'Le Freak', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (283, 5, 278, 'MacArthur Park Suite', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');
INSERT INTO "albums" ("albumid", "genreid", "artistid", "title", "price", "albumarturl") VALUES (284, 5, 277, 'Ring My Bell', CAST(8.99 AS Numeric(10, 2)), '/placeholder.gif');

DROP TABLE IF EXISTS "orderdetails";

CREATE TABLE "orderdetails"(
	"orderdetailid" SERIAL PRIMARY KEY NOT NULL,
	"orderid" int NOT NULL,
	"albumid" int NOT NULL,
	"quantity" int NOT NULL,
	"unitprice" numeric(10, 2) NOT NULL);

DROP TABLE IF EXISTS "carts";

CREATE TABLE "carts"(
	"recordid" SERIAL PRIMARY KEY NOT NULL,
	"cartid" varchar(50) NOT NULL,
	"albumid" int NOT NULL,
	"count" int NOT NULL,
	"datecreated" timestamptz NOT NULL);

DROP TABLE IF EXISTS "users";

CREATE TABLE "users"(
	"userid" SERIAL PRIMARY KEY NOT NULL,
	"username" varchar(200) NOT NULL,
	"email" varchar(200) NOT NULL,
	"password" varchar(200) NOT NULL,
	"role" varchar(50) NOT NULL);

--Password is admin
INSERT INTO "users" ("username", "email", "password", "role") VALUES ('admin', 'admin@example@com', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 'admin');

CREATE VIEW "albumdetails"
as
select a."albumid", a."albumarturl", a."price", a."title", at."name" as "artist", g."name" as "genre"
from "albums" a
join "artists" at on at."artistid" = a."artistid"
join "genres" g on g."genreid" = a."genreid";

CREATE VIEW "cartdetails"
as
select c."cartid", c."count", a."title" as "albumtitle", a."albumid" as "albumid", a."price"
from "carts" c
join "albums" a on c."albumid" = a."albumid";

CREATE VIEW "bestsellers"
as
select a."albumid", a."title", a."albumarturl", Count(*) as "count"
from "albums" as a
inner join "orderdetails" as o on a."albumid" = o."albumid"
group by a."albumid", a."title", a."albumarturl"
order by "count" DESC
LIMIT 5;

CREATE USER suave WITH ENCRYPTED Password '1234';
GRANT USAGE ON SCHEMA public to suave;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON TABLES TO suave;

GRANT CONNECT ON DATABASE "suavemusicstore" to suave;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO suave;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO suave;

