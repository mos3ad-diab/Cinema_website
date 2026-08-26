using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema_website.Migrations
{
    /// <inheritdoc />
    public partial class seed_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {// =========================
         // Categories
         // =========================
            migrationBuilder.Sql("""
        INSERT INTO Categories (Name)
        VALUES
        ('Action'),
        ('Comedy'),
        ('Horror'),
        ('Sci-Fi'),
        ('Drama'),
        ('Adventure');
    """);

            // =========================
            // Cinemas
            // =========================
            migrationBuilder.Sql("""
        INSERT INTO Cinemas (Name, Img)
        VALUES
        ('Cairo Cinema', 'cairo.jpg'),
        ('Galaxy Cinema', 'galaxy.jpg'),
        ('Stars Cinema', 'stars.jpg'),
        ('Nile Cinema', 'nile.jpg'),
        ('Royal Cinema', 'royal.jpg');
    """);

            // =========================
            // Actors
            // =========================
            migrationBuilder.Sql("""
        INSERT INTO Actors (Name, Img)
        VALUES
        ('Leonardo DiCaprio', 'leo.jpg'),
        ('Tom Hardy', 'tom.jpg'),
        ('Christian Bale', 'bale.jpg'),
        ('Robert Downey Jr.', 'rdj.jpg'),
        ('Keanu Reeves', 'keanu.jpg'),
        ('Ryan Reynolds', 'ryan.jpg'),
        ('Joaquin Phoenix', 'joaquin.jpg'),
        ('Cillian Murphy', 'cillian.jpg');
    """);

            // =========================
            // Movies
            // =========================
            migrationBuilder.Sql("""
        INSERT INTO Movies
        (Name, Description, Status, Date_Time, MainImg, CategoryId, CinemaId)
        VALUES

        ('Inception',
        'A thief who steals corporate secrets through dream-sharing technology.',
        1, '2010-07-16', 'inception.jpg', 4, 1),

        ('The Dark Knight',
        'Batman faces a criminal mastermind known as the Joker.',
        1, '2008-07-18', 'dark-knight.jpg', 5, 2),

        ('The Matrix',
        'A computer hacker discovers that reality is not what it seems.',
        1, '1999-03-31', 'matrix.jpg', 4, 3),

        ('John Wick',
        'An ex-hitman returns to action after a personal tragedy.',
        1, '2014-10-24', 'john-wick.jpg', 1, 4),

        ('Deadpool',
        'A former special forces operative becomes a wisecracking superhero.',
        1, '2016-02-12', 'deadpool.jpg', 2, 5),

        ('Joker',
        'A troubled man begins a transformation that changes Gotham City.',
        1, '2019-10-04', 'joker.jpg', 5, 1),

        ('Oppenheimer',
        'The story of the scientist who helped develop the atomic bomb.',
        1, '2023-07-21', 'oppenheimer.jpg', 5, 2),

        ('Mad Max Fury Road',
        'A woman rebels against a tyrannical ruler in a post-apocalyptic world.',
        1, '2015-05-15', 'mad-max.jpg', 6, 3),

        ('The Revenant',
        'A frontiersman fights for survival after being left for dead.',
        0, '2015-12-25', 'revenant.jpg', 6, 4),

        ('Avengers Endgame',
        'The Avengers attempt to undo the devastating events of the past.',
        0, '2019-04-26', 'endgame.jpg', 1, 5);
    """);

            // =========================
            // MovieActors
            // =========================
            migrationBuilder.Sql("""
        INSERT INTO MovieActors (MovieId, ActorId)
        VALUES
        -- Inception
        (1, 1),
        (1, 2),

        -- The Dark Knight
        (2, 3),
        (2, 7),

        -- The Matrix
        (3, 5),

        -- John Wick
        (4, 5),

        -- Deadpool
        (5, 6),

        -- Joker
        (6, 7),

        -- Oppenheimer
        (7, 8),

        -- Mad Max
        (8, 2),

        -- The Revenant
        (9, 1),

        -- Avengers Endgame
        (10, 4);
    """);

            // =========================
            // Sub Images
            // =========================
            migrationBuilder.Sql("""
        INSERT INTO SubImgs (Sub_Img, MovieId)
        VALUES

        ('inception1.jpg', 1),
        ('inception2.jpg', 1),

        ('dark-knight1.jpg', 2),
        ('dark-knight2.jpg', 2),

        ('matrix1.jpg', 3),
        ('matrix2.jpg', 3),

        ('john-wick1.jpg', 4),
        ('john-wick2.jpg', 4),

        ('deadpool1.jpg', 5),
        ('deadpool2.jpg', 5),

        ('joker1.jpg', 6),
        ('joker2.jpg', 6),

        ('oppenheimer1.jpg', 7),
        ('oppenheimer2.jpg', 7),

        ('mad-max1.jpg', 8),
        ('mad-max2.jpg', 8),

        ('revenant1.jpg', 9),
        ('revenant2.jpg', 9),

        ('endgame1.jpg', 10),
        ('endgame2.jpg', 10);
    """);
        }


        

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // =========================
            // Sub Images
            // =========================
            migrationBuilder.Sql("""
        DELETE FROM SubImgs
        WHERE MovieId IN (1,2,3,4,5,6,7,8,9,10);
    """);

            // =========================
            // Movie Actors
            // =========================
            migrationBuilder.Sql("""
        DELETE FROM MovieActors
        WHERE MovieId IN (1,2,3,4,5,6,7,8,9,10);
    """);

            // =========================
            // Movies
            // =========================
            migrationBuilder.Sql("""
        DELETE FROM Movies
        WHERE Id IN (1,2,3,4,5,6,7,8,9,10);
    """);

            // =========================
            // Actors
            // =========================
            migrationBuilder.Sql("""
        DELETE FROM Actors
        WHERE Id IN (1,2,3,4,5,6,7,8);
    """);

            // =========================
            // Categories
            // =========================
            migrationBuilder.Sql("""
        DELETE FROM Categories
        WHERE Id IN (1,2,3,4,5,6);
    """);

            // =========================
            // Cinemas
            // =========================
            migrationBuilder.Sql("""
        DELETE FROM Cinemas
        WHERE Id IN (1,2,3,4,5);
    """);

        }
    }
}
