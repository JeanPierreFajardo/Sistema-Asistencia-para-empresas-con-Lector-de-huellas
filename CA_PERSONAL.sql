CREATE DATABASE CA_PERSONAL;
GO

USE CA_PERSONAL;
GO

CREATE TABLE Horario (
    Id_Hor INT PRIMARY KEY IDENTITY(1,1),
    HorEntrada TIME,
    MinTolerancia TIME,
    HorLimite TIME,
    HorSalida TIME
);
GO

INSERT INTO Horario (HorEntrada, MinTolerancia, HorLimite, HorSalida)
VALUES ('08:00:00', '08:15:00', '10:30:00', '17:45:00');
GO

CREATE TABLE TipoDoc (
    IdTipo INT PRIMARY KEY IDENTITY(1,1),
    NombreTipo VARCHAR(50),
    Serie VARCHAR(10),
    Numero_T INT
);
GO

CREATE TABLE Distrito (
    Id_Distrito INT PRIMARY KEY IDENTITY(1,1),
    Distrito VARCHAR(50)
);
GO

CREATE TABLE Rol (
    Id_Rol INT PRIMARY KEY IDENTITY(1,1),
    NomRol VARCHAR(50)
);
GO
INSERT INTO Rol (NomRol) VALUES ('ADMINISTRADOR');
INSERT INTO Rol (NomRol) VALUES ('TRABAJADOR');
INSERT INTO Rol (NomRol) VALUES ('SOLDADOR');
INSERT INTO Rol (NomRol) VALUES ('INGENIERO');
INSERT INTO Rol (NomRol) VALUES ('MAESTRO DE OBRA');
GO

CREATE TABLE PUsuario (
    Id_Usu INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(50),    
    Avatar VARBINARY(MAX),
    NomUsuario VARCHAR(50),
    Password VARCHAR(100),
    Id_Rol INT,
    FOREIGN KEY (Id_Rol) REFERENCES Rol(Id_Rol)
);
GO
select * from PUsuario

CREATE TABLE Personal (
    Id_Pernl INT PRIMARY KEY IDENTITY(1,1),
    DniPer VARCHAR(8),
    Nombre VARCHAR(50),
    ApellidoPaterno VARCHAR(50),
    ApellidoMaterno VARCHAR(50),
    Fec_Naci DATE,
    Sexo VARCHAR(10),
    Domicilio VARCHAR(100),
    Correo VARCHAR(100),
    Celular VARCHAR(9),
    Id_Rol INT,
    Foto VARBINARY(MAX),
    Id_Distrito INT,
    HuellaDactilar VARBINARY(MAX),
    EstadoPers VARCHAR(20) CHECK (EstadoPers IN ('Activo', 'Desactivo')),
    FOREIGN KEY (Id_Rol) REFERENCES Rol(Id_Rol),
    FOREIGN KEY (Id_Distrito) REFERENCES Distrito(Id_Distrito)
);
GO


CREATE TABLE AsistenciaPersonal (
    Id_Asis INT PRIMARY KEY IDENTITY(1,1),
    Id_Pernl INT,
    FechaAsis DATE,
    Nombre_dia VARCHAR(20),
    HorIngreso TIME,
    HorSalida TIME,
    Total_HrsTrabajado TIME,    
    EstadoAsis VARCHAR(20) CHECK (EstadoAsis IN ('Asistido', 'Tardanza','Falto')),    
    FOREIGN KEY (Id_Pernl) REFERENCES Personal(Id_Pernl)
);
GO

CREATE TABLE Justificacion (
    Id_Justi INT PRIMARY KEY IDENTITY(1,1),
    Id_Pernl INT,
    PrincipalMotivo VARCHAR(100),
    Detalle_Justi TEXT,
    FechaJusti DATE,
    EstadoJus VARCHAR(20) CHECK (EstadoJus IN ('Aprobado', 'Desaprobado')),
    Fecha_Emi DATE,
    FOREIGN KEY (Id_Pernl) REFERENCES Personal(Id_Pernl)
);
GO

-- PROCEDIMIENTOS ALMACENADOS
CREATE PROCEDURE ObtenerDatosPersonales
AS
BEGIN
    SELECT 
        p.DniPer AS DNI,
        p.Nombre + ' ' + p.ApellidoPaterno + ' ' + p.ApellidoMaterno AS [Nombre Completo],
        p.Fec_Naci AS [Fecha Nacim.],
        p.Celular AS [Nro Celular],
        r.NomRol AS Cargo,
        p.EstadoPers AS Estado
    FROM Personal p
    INNER JOIN Rol r ON p.Id_Rol = r.Id_Rol
    WHERE p.EstadoPers = 'Activo';
END
GO

CREATE PROCEDURE ObtenerTodosLosDatosPersonales
AS
BEGIN
    SELECT 
        p.DniPer AS DNI,
        p.Nombre + ' ' + p.ApellidoPaterno + ' ' + p.ApellidoMaterno AS [Nombre Completo],
        p.Fec_Naci AS [Fecha Nacim.],
        p.Celular AS [Nro Celular],
        r.NomRol AS Cargo,
        p.EstadoPers AS Estado
    FROM Personal p
    INNER JOIN Rol r ON p.Id_Rol = r.Id_Rol;
END
GO

CREATE PROCEDURE InsertarJustificacion
    @Id_Pernl INT,
    @PrincipalMotivo VARCHAR(100),
    @Detalle_Justi TEXT,
    @FechaJusti DATE
AS
BEGIN
    DECLARE @FechaEmi DATE = GETDATE();

    INSERT INTO Justificacion (Id_Pernl, PrincipalMotivo, Detalle_Justi, FechaJusti, EstadoJus, Fecha_Emi)
    VALUES (@Id_Pernl, @PrincipalMotivo, @Detalle_Justi, @FechaJusti, 'Desaprobado', @FechaEmi);
END
GO

CREATE PROCEDURE ObtenerJustificaciones
AS
BEGIN
    SELECT 
        p.Nombre + ' ' + p.ApellidoPaterno + ' ' + p.ApellidoMaterno AS 'Nombre del Personal',
        j.PrincipalMotivo AS 'Motivo',
        j.Fecha_Emi AS 'Fecha Emision',
        j.FechaJusti AS 'Fecha Justificacion',
        j.EstadoJus AS 'Estado'
    FROM Justificacion j
    INNER JOIN Personal p ON j.Id_Pernl = p.Id_Pernl;
END
GO

CREATE PROCEDURE ObtenerDatosJustificacionPorNombre
    @Nombre VARCHAR(50),
    @ApellidoPaterno VARCHAR(50),
    @ApellidoMaterno VARCHAR(50)
AS
BEGIN
    SELECT 
        j.Id_Pernl,
        p.DniPer,
        p.Nombre,
        p.ApellidoPaterno,
        p.ApellidoMaterno,
        (p.Nombre + ' ' + p.ApellidoPaterno + ' ' + p.ApellidoMaterno) AS NombreCompleto,
        j.Detalle_Justi,
        j.PrincipalMotivo,
        j.FechaJusti
    FROM Justificacion j
    INNER JOIN Personal p ON j.Id_Pernl = p.Id_Pernl
    WHERE p.Nombre = @Nombre AND p.ApellidoPaterno = @ApellidoPaterno AND p.ApellidoMaterno = @ApellidoMaterno;
END
GO

CREATE PROCEDURE sp_CargarAsistenciaActual
AS
BEGIN
    DECLARE @FechaActual DATE = GETDATE();

    SELECT 
        P.DniPer AS 'Dni',
        P.Nombre + ' ' + P.ApellidoPaterno + ' ' + P.ApellidoMaterno AS 'Nombre Completo',
        AP.FechaAsis AS 'Fecha Asistencia',
        AP.Nombre_dia AS 'Dia',
        AP.HorIngreso AS 'Hora Ingreso',
        AP.HorSalida AS 'Hora Salida',
        AP.EstadoAsis AS 'Estado'
    FROM Personal P
    JOIN AsistenciaPersonal AP ON P.Id_Pernl = AP.Id_Pernl
    WHERE AP.FechaAsis = @FechaActual;
END
GO

CREATE PROCEDURE sp_CargarAsistencia
AS
BEGIN
    SELECT 
        P.DniPer AS 'Dni',
        P.Nombre + ' ' + P.ApellidoPaterno + ' ' + P.ApellidoMaterno AS 'Nombre Completo',
        AP.FechaAsis AS 'Fecha Asistencia',
        AP.Nombre_dia AS 'Dia',
        AP.HorIngreso AS 'Hora Ingreso',
        AP.HorSalida AS 'Hora Salida',
        AP.EstadoAsis AS 'Estado'
    FROM Personal P
    JOIN AsistenciaPersonal AP ON P.Id_Pernl = AP.Id_Pernl;
END
GO


CREATE PROCEDURE ObtenerDatosInforme
    @Nombre VARCHAR(50),
    @ApellidoPaterno VARCHAR(50),
    @ApellidoMaterno VARCHAR(50),
    @FechaSeleccionada DATE
AS
BEGIN
    SELECT
        AP.FechaAsis AS 'Fecha',
        AP.Nombre_dia AS 'Dia',
        AP.HorIngreso AS 'Ingreso',
        AP.HorSalida AS 'Salida',
        AP.EstadoAsis AS 'Estado_Asistencia',
        CONVERT(VARCHAR(8), DATEADD(SECOND, DATEDIFF(SECOND, AP.HorIngreso, AP.HorSalida), 0), 108) AS 'Total_Hrs',
        CASE 
            WHEN AP.EstadoAsis = 'Asistido' THEN '---'  -- No tiene justificaci n si est  asistido
            WHEN AP.EstadoAsis IN ('Tardanza', 'Falto') THEN
                ISNULL(
                    (SELECT J.EstadoJus 
                     FROM Justificacion J 
                     WHERE J.Id_Pernl = AP.Id_Pernl 
                     AND J.FechaJusti = AP.FechaAsis), 
                    'NO TIENE JUSTIFICACION' -- Mostrar "NO TIENE JUSTIFICACION" si no tiene justificaci n
                )
            ELSE '---'
        END AS 'Justificación'
    FROM AsistenciaPersonal AP
    JOIN Personal P ON AP.Id_Pernl = P.Id_Pernl
    WHERE P.Nombre = @Nombre 
    AND P.ApellidoPaterno = @ApellidoPaterno 
    AND P.ApellidoMaterno = @ApellidoMaterno 
    AND AP.FechaAsis >= @FechaSeleccionada
    ORDER BY AP.FechaAsis;
END
GO





CREATE PROCEDURE ObtenerDatosMensuales
    @FechaSeleccionada DATE
AS
BEGIN
    SELECT
        P.Nombre + ' ' + P.ApellidoPaterno + ' ' + P.ApellidoMaterno AS 'Nombre Completo',
        AP.FechaAsis AS 'Fecha Asistencia',
        AP.Nombre_dia AS 'Dia',
        AP.HorIngreso AS 'Hora Ingreso',
        AP.HorSalida AS 'Hora Salida',
        AP.EstadoAsis AS 'Estado',
        CASE 
			WHEN AP.EstadoAsis = 'Asistido' THEN '-'  
			WHEN AP.EstadoAsis IN ('Tardanza', 'Falto') THEN 
				ISNULL(
					(SELECT EstadoJus 
					 FROM Justificacion J 
					 WHERE J.Id_Pernl = AP.Id_Pernl 
					 AND J.FechaJusti = AP.FechaAsis), 
					'NO TIENE JUSTIFICACIÓN'
				)
			ELSE 'N/A' 
		END AS 'Justificación'
    FROM Personal P
    JOIN AsistenciaPersonal AP ON P.Id_Pernl = AP.Id_Pernl
    WHERE DATEPART(MONTH, AP.FechaAsis) = DATEPART(MONTH, @FechaSeleccionada) 
    AND DATEPART(YEAR, AP.FechaAsis) = DATEPART(YEAR, @FechaSeleccionada)
    ORDER BY AP.FechaAsis;
END
GO


INSERT INTO AsistenciaPersonal (Id_Pernl, FechaAsis, Nombre_dia, EstadoAsis)
SELECT p.Id_Pernl, CAST(GETDATE() AS DATE), DATENAME(WEEKDAY, GETDATE()), 'Falto'
FROM Personal p
LEFT JOIN AsistenciaPersonal a 
ON p.Id_Pernl = a.Id_Pernl AND a.FechaAsis = CAST(GETDATE() AS DATE)
WHERE a.Id_Pernl IS NULL;
GO

CREATE PROCEDURE sp_RegistrarFaltasAutomaticamente
AS
BEGIN
    DECLARE @FechaHoy DATE = CAST(GETDATE() AS DATE);

    INSERT INTO AsistenciaPersonal (Id_Pernl, FechaAsis, Nombre_dia, EstadoAsis)
    SELECT p.Id_Pernl, @FechaHoy, DATENAME(WEEKDAY, @FechaHoy), 'Falto'
    FROM Personal p
    LEFT JOIN AsistenciaPersonal a 
    ON p.Id_Pernl = a.Id_Pernl AND a.FechaAsis = @FechaHoy
    WHERE a.Id_Pernl IS NULL;
END
GO


