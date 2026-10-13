-- =====================================================
-- ParkFlow - Datos de prueba (para parkflow_schema.sql)
-- Ejecutar DESPUES del esquema y UNA sola vez (si repites, da llave duplicada).
-- =====================================================
USE `proyecto_integrador`;

-- 1. Usuarios (UPPER para que el hash coincida con Convert.ToHexString de C#)
INSERT INTO usuario (usuario, password, rol) VALUES
('admin',   UPPER(SHA2('admin123', 256)),   'ADMIN'),
('portero', UPPER(SHA2('portero123', 256)), 'PORTERO');

-- 2. Personas (5 residentes, 3 visitantes)
INSERT INTO persona (documento, tipo_documento, nombre, apellido, telefono, correo, tipo_persona, movilidad_reducida) VALUES
('1001','CC','Ana','Pérez','3001112222','ana@mail.com','RESIDENTE',0),
('1002','CC','Luis','Gómez','3003334444','luis@mail.com','RESIDENTE',1),
('1003','CC','Carla','Mendoza','3015556666','carla@mail.com','RESIDENTE',0),
('1004','CE','Pedro','Ramírez','3027778888',NULL,'RESIDENTE',0),
('1005','CC','Sofía','Torres','3039990000','sofia@mail.com','RESIDENTE',0),
('2001','CC','Marta','Ruiz','3041112222','marta@mail.com','VISITANTE',0),
('2002','TI','Jorge','Castro','3053334444',NULL,'VISITANTE',0),
('2003','CC','Elena','Díaz','3065556666','elena@mail.com','VISITANTE',0);

-- 3. Zonas (medidas en metros)
INSERT INTO zona (nombre, ancho, largo) VALUES
('Zona A', 15, 6),
('Zona B', 9, 6);

-- 4. Viviendas (Ana tiene dos apartamentos: prueba del 1 a muchos)
INSERT INTO vivienda (persona_documento, torre, piso, apartamento) VALUES
('1001',1,3,301),   -- id 1
('1001',1,5,502),   -- id 2
('1002',2,1,102),   -- id 3
('1003',2,4,403),   -- id 4
('1004',3,2,201),   -- id 5
('1005',3,6,601);   -- id 6

-- 5. Vehículos (Ana tiene un carro y una moto)
INSERT INTO vehiculo (placa, tipo_vehiculo, persona_documento, modelo, marca) VALUES
('ABC123','CARRO','1001','2020','Mazda'),
('DEF456','MOTO', '1001','2019','Yamaha'),
('XYZ987','CARRO','1002','2018','Renault'),
('GHI789','CARRO','1003','2022','Chevrolet'),
('JKL321','CARRO','1004','2017','Kia'),
('MNO654','CARRO','1005','2021','Toyota'),
('VIS001','CARRO','2001','2016','Nissan'),
('VIS002','MOTO', '2002','2015','Honda');

-- 6. Plazas (10). persona_documento solo en plazas fijas; vehiculo_placa NULL = libre
INSERT INTO parqueo (id_zona, codigo, nivel, coord_x, coord_y, coord_z, tipo, persona_documento) VALUES
((SELECT id_zona FROM zona WHERE nombre = 'Zona A'), 'A-01', 1,  0,  0, 0, 'RESIDENTE',          '1001'),
((SELECT id_zona FROM zona WHERE nombre = 'Zona A'), 'A-02', 1,  3,  0, 0, 'RESIDENTE',          NULL),
((SELECT id_zona FROM zona WHERE nombre = 'Zona A'), 'A-03', 1,  6,  0, 0, 'MOVILIDAD_REDUCIDA', NULL),
((SELECT id_zona FROM zona WHERE nombre = 'Zona A'), 'A-04', 1,  9,  0, 0, 'RESIDENTE',          '1003'),
((SELECT id_zona FROM zona WHERE nombre = 'Zona A'), 'A-05', 1, 12,  0, 0, 'RESIDENTE',          NULL),
((SELECT id_zona FROM zona WHERE nombre = 'Zona B'), 'B-01', 1,  0, 10, 0, 'VISITANTE',          NULL),
((SELECT id_zona FROM zona WHERE nombre = 'Zona B'), 'B-02', 1,  3, 10, 0, 'VISITANTE',          NULL),
((SELECT id_zona FROM zona WHERE nombre = 'Zona B'), 'B-03', 1,  6, 10, 0, 'VISITANTE',          NULL),
((SELECT id_zona FROM zona WHERE nombre = 'Zona A'), 'A-06', 2,  0,  0, 3, 'RESIDENTE',          '1005'),
((SELECT id_zona FROM zona WHERE nombre = 'Zona A'), 'A-07', 2,  3,  0, 3, 'RESIDENTE',          NULL);

-- 7. Ocupar algunas plazas (estado actual)
UPDATE parqueo SET vehiculo_placa = 'ABC123' WHERE codigo = 'A-01';
UPDATE parqueo SET vehiculo_placa = 'XYZ987' WHERE codigo = 'A-03';  -- Luis, movilidad reducida
UPDATE parqueo SET vehiculo_placa = 'GHI789' WHERE codigo = 'A-04';
UPDATE parqueo SET vehiculo_placa = 'VIS001' WHERE codigo = 'B-01';

-- 8. Visitas entre personas (la primera sigue adentro: fecha_salida NULL)
INSERT INTO registro (persona_visitante, persona_visitada, vivienda_id_vivienda, fecha_ingreso, fecha_salida) VALUES
('2001','1001', 1, '2026-10-11 14:00:00', NULL),
('2002','1003', 4, '2026-10-10 09:30:00', '2026-10-10 11:45:00'),
('2003','1002', 3, '2026-10-09 16:00:00', '2026-10-09 18:20:00');

-- 9. Historial de estancias de vehículos
-- 9a. Estancias abiertas (coinciden con las plazas ocupadas del paso 7)
INSERT INTO estancia (vehiculo_placa, id_parqueo, fecha_llegada, fecha_ocupacion, fecha_salida) VALUES
('ABC123', (SELECT id_parqueo FROM parqueo WHERE codigo = 'A-01'), '2026-10-11 07:55:00', '2026-10-11 07:58:00', NULL),
('XYZ987', (SELECT id_parqueo FROM parqueo WHERE codigo = 'A-03'), '2026-10-11 08:40:00', '2026-10-11 08:41:30', NULL),
('GHI789', (SELECT id_parqueo FROM parqueo WHERE codigo = 'A-04'), '2026-10-11 09:10:00', '2026-10-11 09:12:00', NULL),
('VIS001', (SELECT id_parqueo FROM parqueo WHERE codigo = 'B-01'), '2026-10-11 14:00:00', '2026-10-11 14:03:00', NULL);

-- 9b. Un vehículo esperando en la fila (sin plaza todavía)
INSERT INTO estancia (vehiculo_placa, id_parqueo, fecha_llegada, fecha_ocupacion, fecha_salida) VALUES
('VIS002', NULL, '2026-10-11 14:20:00', NULL, NULL);

-- 9c. Estancias cerradas (sirven para tiempos de espera, permanencia y flujo)
INSERT INTO estancia (vehiculo_placa, id_parqueo, fecha_llegada, fecha_ocupacion, fecha_salida) VALUES
('ABC123', (SELECT id_parqueo FROM parqueo WHERE codigo = 'A-01'), '2026-10-10 07:50:00', '2026-10-10 07:52:00', '2026-10-10 19:00:00'),
('DEF456', (SELECT id_parqueo FROM parqueo WHERE codigo = 'A-02'), '2026-10-10 18:00:00', '2026-10-10 18:02:00', '2026-10-11 06:30:00'),
('JKL321', (SELECT id_parqueo FROM parqueo WHERE codigo = 'A-05'), '2026-10-10 19:15:00', '2026-10-10 19:20:00', '2026-10-11 07:10:00'),
('MNO654', (SELECT id_parqueo FROM parqueo WHERE codigo = 'A-06'), '2026-10-10 17:30:00', '2026-10-10 17:31:00', '2026-10-11 06:45:00'),
('VIS002', (SELECT id_parqueo FROM parqueo WHERE codigo = 'B-02'), '2026-10-10 09:30:00', '2026-10-10 09:36:00', '2026-10-10 11:45:00');

-- 10. Grafo de rutas
-- 10a. Nodos de infraestructura
INSERT INTO nodo_ruta (nombre, tipo, nivel, coord_x, coord_y, coord_z) VALUES
('ENTRADA',    'ENTRADA', 1, -6, 5, 0),
('SALIDA',     'SALIDA',  1, -6, 8, 0),
('PASILLO_A1', 'CRUCE',   1,  6, 2, 0),
('PASILLO_B1', 'CRUCE',   1,  3, 8, 0),
('RAMPA_N1',   'RAMPA',   1, 14, 2, 0),
('RAMPA_N2',   'RAMPA',   2, 14, 2, 3),
('PASILLO_A2', 'CRUCE',   2, 1.5, 2, 3);

-- 10b. Un nodo por cada plaza (con las mismas coordenadas)
INSERT INTO nodo_ruta (nombre, tipo, nivel, coord_x, coord_y, coord_z, id_parqueo)
SELECT codigo, 'PLAZA', nivel, coord_x, coord_y, coord_z, id_parqueo FROM parqueo;

-- 10c. Aristas de infraestructura (distancia se calcula abajo)
INSERT INTO arista_ruta (nodo_origen, nodo_destino, distancia) VALUES
((SELECT id_nodo FROM nodo_ruta WHERE nombre = 'ENTRADA'),    (SELECT id_nodo FROM nodo_ruta WHERE nombre = 'PASILLO_A1'), 0),
((SELECT id_nodo FROM nodo_ruta WHERE nombre = 'ENTRADA'),    (SELECT id_nodo FROM nodo_ruta WHERE nombre = 'PASILLO_B1'), 0),
((SELECT id_nodo FROM nodo_ruta WHERE nombre = 'PASILLO_A1'), (SELECT id_nodo FROM nodo_ruta WHERE nombre = 'RAMPA_N1'),   0),
((SELECT id_nodo FROM nodo_ruta WHERE nombre = 'RAMPA_N1'),   (SELECT id_nodo FROM nodo_ruta WHERE nombre = 'RAMPA_N2'),   0),
((SELECT id_nodo FROM nodo_ruta WHERE nombre = 'RAMPA_N2'),   (SELECT id_nodo FROM nodo_ruta WHERE nombre = 'PASILLO_A2'), 0),
((SELECT id_nodo FROM nodo_ruta WHERE nombre = 'PASILLO_B1'), (SELECT id_nodo FROM nodo_ruta WHERE nombre = 'SALIDA'),     0),
((SELECT id_nodo FROM nodo_ruta WHERE nombre = 'PASILLO_A1'), (SELECT id_nodo FROM nodo_ruta WHERE nombre = 'SALIDA'),     0);

-- 10d. Cada plaza se conecta a su pasillo
INSERT INTO arista_ruta (nodo_origen, nodo_destino, distancia)
SELECT c.id_nodo, p.id_nodo, 0
FROM nodo_ruta p JOIN nodo_ruta c ON c.nombre = 'PASILLO_A1'
WHERE p.tipo = 'PLAZA' AND p.nombre IN ('A-01','A-02','A-03','A-04','A-05');

INSERT INTO arista_ruta (nodo_origen, nodo_destino, distancia)
SELECT c.id_nodo, p.id_nodo, 0
FROM nodo_ruta p JOIN nodo_ruta c ON c.nombre = 'PASILLO_B1'
WHERE p.tipo = 'PLAZA' AND p.nombre IN ('B-01','B-02','B-03');

INSERT INTO arista_ruta (nodo_origen, nodo_destino, distancia)
SELECT c.id_nodo, p.id_nodo, 0
FROM nodo_ruta p JOIN nodo_ruta c ON c.nombre = 'PASILLO_A2'
WHERE p.tipo = 'PLAZA' AND p.nombre IN ('A-06','A-07');

-- 10e. Distancia euclidiana entre cada par de nodos (incluye la diferencia de nivel en Z)
UPDATE arista_ruta r
JOIN nodo_ruta a ON a.id_nodo = r.nodo_origen
JOIN nodo_ruta b ON b.id_nodo = r.nodo_destino
SET r.distancia = ROUND(SQRT(POW(a.coord_x - b.coord_x, 2)
                           + POW(a.coord_y - b.coord_y, 2)
                           + POW(a.coord_z - b.coord_z, 2)), 2)
WHERE r.id_arista > 0;
