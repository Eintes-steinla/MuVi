USE master
GO
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'MuVi')
    DROP DATABASE MuVi;
GO
CREATE DATABASE MuVi
GO
USE MuVi
GO

-- =============================================
-- BẢNG QUỐC GIA
-- Mục đích: Lưu trữ thông tin các quốc gia sản xuất phim
-- =============================================
CREATE TABLE Countries (
    CountryID INT PRIMARY KEY IDENTITY(1,1),      -- Mã quốc gia (khóa chính, tự động tăng)
    CountryName NVARCHAR(100) NOT NULL UNIQUE,    -- Tên quốc gia (không trùng lặp, bắt buộc nhập)
    CountryCode NVARCHAR(10)                      -- Mã ISO quốc gia (VD: VN, KR, JP, US)
);

-- =============================================
-- BẢNG THỂ LOẠI PHIM
-- Mục đích: Lưu trữ các thể loại phim (hành động, tình cảm, hài...)
-- =============================================
CREATE TABLE Genres (
    GenreID INT PRIMARY KEY IDENTITY(1,1),        -- Mã thể loại (khóa chính, tự động tăng)
    GenreName NVARCHAR(100) NOT NULL UNIQUE,      -- Tên thể loại (không trùng lặp, bắt buộc nhập)
    [Description] NVARCHAR(500)                   -- Mô tả chi tiết về thể loại phim
);

-- =============================================
-- BẢNG DIỄN VIÊN
-- Mục đích: Lưu trữ thông tin chi tiết về các diễn viên
-- =============================================
CREATE TABLE Actors (
    ActorID INT PRIMARY KEY IDENTITY(1,1),        -- Mã diễn viên (khóa chính, tự động tăng)
    ActorName NVARCHAR(100) NOT NULL,             -- Tên diễn viên (bắt buộc nhập)
    Bio NVARCHAR(MAX),                            -- Tiểu sử, giới thiệu về diễn viên
    PhotoPath NVARCHAR(500),                      -- Đường dẫn ảnh đại diện của diễn viên
    DateOfBirth DATE,                             -- Ngày sinh của diễn viên
    Nationality NVARCHAR(100),                    -- Quốc tịch của diễn viên
    CreatedAt DATETIME DEFAULT GETDATE()          -- Thời gian tạo bản ghi (mặc định là thời gian hiện tại)
);

-- =============================================
-- BẢNG PHIM CHÍNH
-- Mục đích: Lưu trữ thông tin chi tiết về các bộ phim (cả phim lẻ và phim bộ)
-- =============================================
CREATE TABLE Movies (
    MovieID INT PRIMARY KEY IDENTITY(1,1),        -- Mã phim (khóa chính, tự động tăng)
    Title NVARCHAR(255) NOT NULL,                 -- Tên phim (bắt buộc nhập)
    MovieType NVARCHAR(20) NOT NULL CHECK (MovieType IN (N'Phim lẻ', N'Phim bộ')), -- Loại phim: Phim lẻ hoặc Phim bộ
    CountryID INT,                                -- Mã quốc gia sản xuất (khóa ngoại tới bảng Countries)
    Director NVARCHAR(100),                       -- Tên đạo diễn/tác giả phim
    ReleaseYear INT,                              -- Năm phát hành phim
    [Description] NVARCHAR(MAX),                  -- Mô tả nội dung, cốt truyện phim
    PosterPath NVARCHAR(500),                     -- Đường dẫn ảnh poster/áp phích phim
    TrailerURL NVARCHAR(500),                     -- Đường dẫn URL trailer/teaser phim
    VideoPath NVARCHAR(500),                      -- Đường dẫn file video phim - CHỈ dùng cho phim lẻ (phim bộ dùng VideoPath trong bảng Episodes)
	Duration INT,                                 -- Thời lượng phim (phút) - CHỈ dùng cho phim lẻ
    TotalEpisodes INT,                            -- Tổng số tập phim - CHỈ dùng cho phim bộ
    Rating DECIMAL(3,2) DEFAULT 0.00 CHECK (Rating BETWEEN 0 AND 10), -- Điểm đánh giá trung bình (thang điểm 0-10)
    ViewCount INT DEFAULT 0,                      -- Tổng số lượt xem phim
    [Status] NVARCHAR(20) DEFAULT N'Đang chiếu' CHECK ([Status] IN (N'Đang chiếu', N'Hoàn thành', N'Sắp chiếu')), -- Trạng thái phim
    CreatedAt DATETIME DEFAULT GETDATE(),         -- Thời gian tạo bản ghi phim
    UpdatedAt DATETIME DEFAULT GETDATE(),         -- Thời gian cập nhật bản ghi gần nhất
    CONSTRAINT FK_Movies_Countries FOREIGN KEY (CountryID) 
        REFERENCES Countries(CountryID) ON DELETE SET NULL -- Nếu xóa quốc gia, CountryID sẽ = NULL
);

-- =============================================
-- BẢNG TẬP PHIM
-- Mục đích: Lưu trữ thông tin từng tập của phim bộ
-- =============================================
CREATE TABLE Episodes (
    EpisodeID INT PRIMARY KEY IDENTITY(1,1),      -- Mã tập phim (khóa chính, tự động tăng)
    MovieID INT NOT NULL,                         -- Mã phim bộ (khóa ngoại tới bảng Movies, bắt buộc)
    EpisodeNumber INT NOT NULL,                   -- Số thứ tự tập (VD: tập 1, 2, 3...)
    Title NVARCHAR(255) NOT NULL,                 -- Tiêu đề của tập phim
    [Description] NVARCHAR(MAX),                  -- Mô tả nội dung tập phim
    Duration INT,                                 -- Thời lượng của tập (phút)
    VideoPath NVARCHAR(500),                      -- Đường dẫn file video của tập phim
    ReleaseDate DATE,                             -- Ngày phát hành tập phim
    ViewCount INT DEFAULT 0,                      -- Số lượt xem của tập phim này
    CreatedAt DATETIME DEFAULT GETDATE(),         -- Thời gian tạo bản ghi
    CONSTRAINT FK_Episodes_Movies FOREIGN KEY (MovieID) 
        REFERENCES Movies(MovieID) ON DELETE CASCADE, -- Xóa phim thì xóa tất cả tập
    CONSTRAINT UQ_Episode UNIQUE (MovieID, EpisodeNumber) -- Mỗi phim không có 2 tập trùng số
);

-- =============================================
-- BẢNG TRUNG GIAN: PHIM - THỂ LOẠI
-- Mục đích: Quan hệ nhiều-nhiều giữa Phim và Thể loại (1 phim có nhiều thể loại, 1 thể loại có nhiều phim)
-- =============================================
CREATE TABLE MovieCategory (
    MovieID INT,                                  -- Mã phim (khóa ngoại tới bảng Movies)
    GenreID INT,                                  -- Mã thể loại (khóa ngoại tới bảng Genres)
    PRIMARY KEY (MovieID, GenreID),              -- Khóa chính kết hợp (mỗi cặp phim-thể loại là duy nhất)
    CONSTRAINT FK_MC_Movies FOREIGN KEY (MovieID) 
        REFERENCES Movies(MovieID) ON DELETE CASCADE, -- Xóa phim thì xóa các liên kết thể loại
    CONSTRAINT FK_MC_Genres FOREIGN KEY (GenreID) 
        REFERENCES Genres(GenreID) ON DELETE CASCADE  -- Xóa thể loại thì xóa các liên kết phim
);

-- =============================================
-- BẢNG TRUNG GIAN: PHIM - DIỄN VIÊN
-- Mục đích: Quan hệ nhiều-nhiều giữa Phim và Diễn viên (1 phim có nhiều diễn viên, 1 diễn viên đóng nhiều phim)
-- =============================================
CREATE TABLE MovieCast (
    MovieID INT,                                  -- Mã phim (khóa ngoại tới bảng Movies)
    ActorID INT,                                  -- Mã diễn viên (khóa ngoại tới bảng Actors)
    RoleName NVARCHAR(100),                       -- Tên vai diễn trong phim (VD: "Nhân vật chính", "Phản diện")
    [Order] INT DEFAULT 0,                        -- Thứ tự ưu tiên (1=diễn viên chính, 2=phụ, càng nhỏ càng quan trọng)
    PRIMARY KEY (MovieID, ActorID),              -- Khóa chính kết hợp (mỗi diễn viên chỉ đóng 1 vai trong 1 phim)
    CONSTRAINT FK_Cast_Movies FOREIGN KEY (MovieID) 
        REFERENCES Movies(MovieID) ON DELETE CASCADE, -- Xóa phim thì xóa thông tin diễn viên tham gia
    CONSTRAINT FK_Cast_Actors FOREIGN KEY (ActorID) 
        REFERENCES Actors(ActorID) ON DELETE CASCADE  -- Xóa diễn viên thì xóa các vai diễn của họ
);

-- =============================================
-- BẢNG NGƯỜI DÙNG
-- Mục đích: Lưu trữ thông tin tài khoản người dùng hệ thống
-- =============================================
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),         -- Mã người dùng (khóa chính, tự động tăng)
    Username NVARCHAR(50) NOT NULL UNIQUE,        -- Tên đăng nhập (duy nhất, bắt buộc)
    [Password] NVARCHAR(255) NOT NULL,            -- Mật khẩu đã mã hóa (bắt buộc)
    Email NVARCHAR(100) UNIQUE,                   -- Email (duy nhất, dùng để khôi phục mật khẩu)
    DateOfBirth DATE,                             -- Ngày sinh của người dùng
    Avatar NVARCHAR(500),                         -- Đường dẫn ảnh đại diện
    [Role] NVARCHAR(20) DEFAULT 'User' CHECK ([Role] IN ('Admin', 'User')), -- Vai trò: Admin (quản trị) hoặc User (người dùng thường)
    IsActive BIT DEFAULT 1,                       -- Trạng thái tài khoản (1=đang hoạt động, 0=bị khóa)
    LastLogin DATETIME,                           -- Thời gian đăng nhập lần cuối
    CreatedAt DATETIME DEFAULT GETDATE(),         -- Thời gian tạo tài khoản
    UpdatedAt DATETIME DEFAULT GETDATE()          -- Thời gian cập nhật thông tin gần nhất
);

-- =============================================
-- BẢNG ĐÁNH GIÁ & BÌNH LUẬN
-- Mục đích: Lưu trữ đánh giá, bình luận của người dùng về phim
-- =============================================
CREATE TABLE Reviews (
    ReviewID INT PRIMARY KEY IDENTITY(1,1),       -- Mã đánh giá (khóa chính, tự động tăng)
    MovieID INT NOT NULL,                         -- Mã phim được đánh giá (khóa ngoại, bắt buộc)
    UserID INT NOT NULL,                          -- Mã người dùng đánh giá (khóa ngoại, bắt buộc)
    Rating INT CHECK (Rating BETWEEN 1 AND 10),   -- Điểm số đánh giá (thang điểm 1-10)
    Comment NVARCHAR(MAX),                        -- Nội dung bình luận, nhận xét về phim
    LikeCount INT DEFAULT 0,                      -- Số lượt thích của bình luận này (từ người dùng khác)
    CreatedAt DATETIME DEFAULT GETDATE(),         -- Thời gian tạo đánh giá
    UpdatedAt DATETIME DEFAULT GETDATE(),         -- Thời gian chỉnh sửa đánh giá gần nhất
    CONSTRAINT FK_Reviews_Movies FOREIGN KEY (MovieID) 
        REFERENCES Movies(MovieID) ON DELETE CASCADE, -- Xóa phim thì xóa tất cả đánh giá
    CONSTRAINT FK_Reviews_Users FOREIGN KEY (UserID) 
        REFERENCES Users(UserID) ON DELETE CASCADE,   -- Xóa user thì xóa tất cả đánh giá của họ
    CONSTRAINT UQ_UserMovie_Review UNIQUE (UserID, MovieID) -- Mỗi user chỉ được đánh giá 1 lần cho mỗi phim
);

-- =============================================
-- BẢNG DANH SÁCH YÊU THÍCH
-- Mục đích: Lưu trữ danh sách phim yêu thích của từng người dùng
-- =============================================
CREATE TABLE Favorites (
    UserID INT,                                   -- Mã người dùng (khóa ngoại tới bảng Users)
    MovieID INT,                                  -- Mã phim yêu thích (khóa ngoại tới bảng Movies)
    AddedAt DATETIME DEFAULT GETDATE(),           -- Thời gian thêm phim vào danh sách yêu thích
    PRIMARY KEY (UserID, MovieID),               -- Khóa chính kết hợp (1 user không thể thêm 1 phim 2 lần)
    CONSTRAINT FK_Fav_Users FOREIGN KEY (UserID) 
        REFERENCES Users(UserID) ON DELETE CASCADE,   -- Xóa user thì xóa danh sách yêu thích của họ
    CONSTRAINT FK_Fav_Movies FOREIGN KEY (MovieID) 
        REFERENCES Movies(MovieID) ON DELETE CASCADE  -- Xóa phim thì xóa khỏi danh sách yêu thích của tất cả user
);

-- =============================================
-- BẢNG LỊCH SỬ XEM PHIM
-- Mục đích: Theo dõi lịch sử xem phim của người dùng, tiến độ xem
-- =============================================
CREATE TABLE ViewHistory (
    HistoryID INT PRIMARY KEY IDENTITY(1,1),      -- Mã lịch sử (khóa chính, tự động tăng)
    UserID INT NOT NULL,                          -- Mã người dùng (khóa ngoại, bắt buộc)
    MovieID INT NOT NULL,                         -- Mã phim đã xem (khóa ngoại, bắt buộc)
    EpisodeID INT,                                -- Mã tập phim (NULL nếu là phim lẻ)
    WatchedAt DATETIME DEFAULT GETDATE(),         -- Thời gian xem gần nhất
    WatchDuration INT DEFAULT 0,                  -- Thời gian đã xem (tính bằng giây) - dùng để tiếp tục xem từ vị trí cũ
    IsCompleted BIT DEFAULT 0,                    -- Đánh dấu đã xem hết phim/tập chưa (1=đã xem hết, 0=chưa)
    CONSTRAINT FK_History_Users FOREIGN KEY (UserID) 
        REFERENCES Users(UserID) ON DELETE CASCADE,   -- Xóa user thì xóa lịch sử xem của họ
    CONSTRAINT FK_History_Movies FOREIGN KEY (MovieID) 
        REFERENCES Movies(MovieID) ON DELETE CASCADE, -- Xóa phim thì xóa lịch sử xem phim đó
    CONSTRAINT FK_History_Episodes FOREIGN KEY (EpisodeID) 
        REFERENCES Episodes(EpisodeID) ON DELETE NO ACTION -- Không tự động xóa khi xóa tập (để giữ lại lịch sử)
);

-- =============================================
-- INDEXES ĐỂ TỐI ƯU HIỆU SUẤT TRUY VẤN
-- Mục đích: Tăng tốc độ tìm kiếm, sắp xếp dữ liệu
-- =============================================

-- Index cho tìm kiếm phim theo tên
CREATE INDEX IX_Movies_Title ON Movies(Title);

-- Index cho lọc phim theo trạng thái
CREATE INDEX IX_Movies_Status ON Movies([Status]);

-- Index cho sắp xếp phim theo điểm đánh giá (giảm dần)
CREATE INDEX IX_Movies_Rating ON Movies(Rating DESC);

-- Index cho sắp xếp phim theo lượt xem (giảm dần)
CREATE INDEX IX_Movies_ViewCount ON Movies(ViewCount DESC);

-- Index cho truy vấn tập phim theo MovieID
CREATE INDEX IX_Episodes_MovieID ON Episodes(MovieID);

-- Index cho truy vấn đánh giá theo phim
CREATE INDEX IX_Reviews_MovieID ON Reviews(MovieID);

-- Index cho truy vấn đánh giá theo người dùng
CREATE INDEX IX_Reviews_UserID ON Reviews(UserID);

-- Index cho truy vấn lịch sử xem theo người dùng
CREATE INDEX IX_ViewHistory_UserID ON ViewHistory(UserID);

-- Index cho truy vấn lịch sử xem theo phim
CREATE INDEX IX_ViewHistory_MovieID ON ViewHistory(MovieID);

-- Index cho tìm kiếm người dùng theo email
CREATE INDEX IX_Users_Email ON Users(Email);

-- Index cho tìm kiếm người dùng theo username
CREATE INDEX IX_Users_Username ON Users(Username);

GO

-- =============================================
-- DỮ LIỆU MẪU
-- =============================================

-- Thêm dữ liệu mẫu cho bảng Countries
INSERT INTO Countries (CountryName, CountryCode) VALUES
(N'Việt Nam', 'VN'),      -- Quốc gia sản xuất phim Việt Nam
(N'Hàn Quốc', 'KR'),      -- Quốc gia sản xuất phim Hàn Quốc
(N'Nhật Bản', 'JP'),      -- Quốc gia sản xuất phim Nhật Bản
(N'Mỹ', 'US'),            -- Quốc gia sản xuất phim Mỹ
(N'Trung Quốc', 'CN'),    -- Quốc gia sản xuất phim Trung Quốc
(N'Thái Lan', 'TH');      -- Quốc gia sản xuất phim Thái Lan

-- Thêm dữ liệu mẫu cho bảng Genres
INSERT INTO Genres (GenreName, [Description]) VALUES
(N'Hành động', N'Phim hành động, võ thuật'),              -- Thể loại phim hành động
(N'Tình cảm', N'Phim tình cảm, lãng mạn'),                -- Thể loại phim tình cảm
(N'Hài', N'Phim hài hước'),                                -- Thể loại phim hài
(N'Kinh dị', N'Phim kinh dị, ma'),                         -- Thể loại phim kinh dị
(N'Khoa học viễn tưởng', N'Phim sci-fi'),                  -- Thể loại phim khoa học viễn tưởng
(N'Hoạt hình', N'Phim hoạt hình, anime'),                  -- Thể loại phim hoạt hình
(N'Tâm lý', N'Phim tâm lý, drama'),                        -- Thể loại phim tâm lý
(N'Trinh thám', N'Phim trinh thám, bí ẩn');                -- Thể loại phim trinh thám

-- Thêm dữ liệu mẫu cho bảng Actors
INSERT INTO Actors (ActorName, Nationality, DateOfBirth) VALUES
(N'Trấn Thành', N'Việt Nam', '1987-02-05'),       -- Diễn viên Việt Nam nổi tiếng
(N'Ninh Dương Lan Ngọc', N'Việt Nam', '1990-06-25'), -- Diễn viên Việt Nam nổi tiếng
(N'Lee Min Ho', N'Hàn Quốc', '1987-06-22'),       -- Diễn viên Hàn Quốc nổi tiếng
(N'Son Ye Jin', N'Hàn Quốc', '1982-01-11');       -- Diễn viên Hàn Quốc nổi tiếng

-- Thêm dữ liệu mẫu cho bảng Users
INSERT INTO Users (Username, [Password], Email, [Role]) VALUES
('admin', 'admin123', 'admin@muvi.com', 'Admin'),  -- Tài khoản quản trị viên
('user1', 'user123', 'user1@gmail.com', 'User');   -- Tài khoản người dùng thường

GO

PRINT N'✅ Tạo database MuVi thành công!'
PRINT N'📊 Bảng đã tạo:'
PRINT N'   - Countries, Genres, Actors'
PRINT N'   - Movies, Episodes'
PRINT N'   - MovieCategory, MovieCast'
PRINT N'   - Users, Reviews, Favorites, ViewHistory'
PRINT N'🔍 Indexes đã được tạo để tối ưu hiệu suất'
PRINT N'📝 Dữ liệu mẫu đã được thêm vào'