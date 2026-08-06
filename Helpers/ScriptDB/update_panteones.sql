-- =========================================================
-- UPDATE MASIVO: nombrePanteon (Parcelas) + Anio de adquisicion (Concesiones.informacionAdicional)
-- Generado automaticamente desde BaseDatosParaCargarReducida.xlsx (hoja PANTEONES)
-- Total registros del Excel: 134 (filas 4 a 137; las filas 1-3 son encabezado/nota)
-- Dividido en lotes de 20 registros, cada uno en su propia transaccion (si un lote falla, se revierte solo ese lote).
-- Validado contra Secciones_5.xlsx: Secciones.nombre = SECC del Excel (ej: '11'), filtrando SIEMPRE por TipoParcelaId = 3 (panteon),
-- ya que existen nombres de seccion duplicados entre tipos (ej: '12', '14', '16', '18', '19', '20', '21', '22' existen tanto
-- en secciones de nichos/fosas como de panteones). Los 19 valores de SECC usados en el Excel de panteones matchean
-- exactamente contra las 19 secciones con TipoParcelaId = 3.
-- =========================================================

USE cemsys;
GO

-- ================= LOTE 1 de 7 (filas Excel 4 a 23) =================
SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- Fila Excel 4: ABBIATTI ROBERTO
UPDATE P SET P.nombrePanteon = N'ABBIATTI ROBERTO' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'11' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 6 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1978') WHERE concesion = 2373;

-- Fila Excel 5: AMUCHASTEGUI OMAR
UPDATE P SET P.nombrePanteon = N'AMUCHASTEGUI OMAR' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'13' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 6 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1980') WHERE concesion = 2375;

-- Fila Excel 6: ANDREU FRANCISCO (c/nichos)
UPDATE P SET P.nombrePanteon = N'ANDREU FRANCISCO (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'10' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 6 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2007') WHERE concesion = 2372;

-- Fila Excel 7: ARCE Y FERREYRA
UPDATE P SET P.nombrePanteon = N'ARCE Y FERREYRA' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'11' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 7 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1980') WHERE concesion = 2383;

-- Fila Excel 8: ARDILES ALMENGOL EUFEMIO   S/PLANO cargar contribuyente
UPDATE P SET P.nombrePanteon = N'ARDILES ALMENGOL EUFEMIO   S/PLANO cargar contribuyente' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'11' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1989') WHERE concesion = 2703;

-- Fila Excel 9: ARIENTI JUAN CARLOS
UPDATE P SET P.nombrePanteon = N'ARIENTI JUAN CARLOS' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'7' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1979') WHERE concesion = 2323;

-- Fila Excel 10: BARRIONUENO Y BOROCIONI
UPDATE P SET P.nombrePanteon = N'BARRIONUENO Y BOROCIONI' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'17' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 3 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1993') WHERE concesion = 2342;

-- Fila Excel 11: BAZAN TERESA, FERNANDO, BRIGIDA Y JOSEFA  (c/nichos)
UPDATE P SET P.nombrePanteon = N'BAZAN TERESA, FERNANDO, BRIGIDA Y JOSEFA  (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'18' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1993') WHERE concesion = 2317;

-- Fila Excel 12: BENITEZ DE VERA MARIA E.
UPDATE P SET P.nombrePanteon = N'BENITEZ DE VERA MARIA E.' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'13' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 7 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1981') WHERE concesion = 2595;

-- Fila Excel 13: BENITEZ ROMAN Y ARGUELLO JUANA VDA. DE VARELA  (c/nichos)
UPDATE P SET P.nombrePanteon = N'BENITEZ ROMAN Y ARGUELLO JUANA VDA. DE VARELA  (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'16' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 4 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1985') WHERE concesion = 2354;

-- Fila Excel 14: BERRINO Y BERTAGNA (c/nichos)
UPDATE P SET P.nombrePanteon = N'BERRINO Y BERTAGNA (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'14' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 2 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1981') WHERE concesion = 2521;

-- Fila Excel 15: BERTOLINI RAQUEL
UPDATE P SET P.nombrePanteon = N'BERTOLINI RAQUEL' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'14' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 9 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1982') WHERE concesion = 2405;

-- Fila Excel 16: BICEGO NORBERTO Y CORNEJO MARIA ESTER (c/nichos)
UPDATE P SET P.nombrePanteon = N'BICEGO NORBERTO Y CORNEJO MARIA ESTER (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'11' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 8 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1986') WHERE concesion = 2393;

-- Fila Excel 17: BOAROTTO ANTONIO Y FLIA (c/cajon a la vista)
UPDATE P SET P.nombrePanteon = N'BOAROTTO ANTONIO Y FLIA (c/cajon a la vista)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'10' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 2 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1974') WHERE concesion = 2326;

-- Fila Excel 18: BOAROTTO CELESTINO (C/cajon a la vista) cargar contribuyente
UPDATE P SET P.nombrePanteon = N'BOAROTTO CELESTINO (C/cajon a la vista) cargar contribuyente' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'2' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 6 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1975') WHERE concesion = 2728;

-- Fila Excel 19: BOAROTTO MIGUEL Y LUIS (C/cajon a la vista)
UPDATE P SET P.nombrePanteon = N'BOAROTTO MIGUEL Y LUIS (C/cajon a la vista)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'2' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1966') WHERE concesion = 2704;

-- Fila Excel 20: BORDI NAZARENO (c/nichos)
UPDATE P SET P.nombrePanteon = N'BORDI NAZARENO (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'10' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 7 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1975') WHERE concesion = 2507;

-- Fila Excel 21: BORJABAD DE DIALE BLANCA ZULEMA
UPDATE P SET P.nombrePanteon = N'BORJABAD DE DIALE BLANCA ZULEMA' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'18' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 8 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1996') WHERE concesion = 2399;

-- Fila Excel 22: BOSOLETTI ROMUALDO
UPDATE P SET P.nombrePanteon = N'BOSOLETTI ROMUALDO' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'12' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 10 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1980') WHERE concesion = 2416;

-- Fila Excel 23: BROCHERO MARIA EVANGELINA
UPDATE P SET P.nombrePanteon = N'BROCHERO MARIA EVANGELINA' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'3' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 3 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2014') WHERE concesion = 2346;

COMMIT TRANSACTION;
PRINT 'Lote 1 de 7 completado (filas Excel 4-23)';
GO

-- ================= LOTE 2 de 7 (filas Excel 24 a 43) =================
SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- Fila Excel 24: BUCCINNA DE PUCCI CATERINA (c/nichos)
UPDATE P SET P.nombrePanteon = N'BUCCINNA DE PUCCI CATERINA (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'18' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 5 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1997') WHERE concesion = 2367;

-- Fila Excel 25: BUSTOS JUAN CARLOS
UPDATE P SET P.nombrePanteon = N'BUSTOS JUAN CARLOS' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'17' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 5 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1993') WHERE concesion = 2366;

-- Fila Excel 26: CAMPREGER (OCAÑA ECILDA) (C/cajón a la vista) cargar contrib
UPDATE P SET P.nombrePanteon = N'CAMPREGER (OCAÑA ECILDA) (C/cajón a la vista) cargar contribuyente' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'1' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 3 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1965') WHERE concesion = 2730;

-- Fila Excel 27: CANTERLE GERARDO
UPDATE P SET P.nombrePanteon = N'CANTERLE GERARDO' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'14' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 3 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1982') WHERE concesion = 2340;

-- Fila Excel 28: CAPELLO Y GIANRE
UPDATE P SET P.nombrePanteon = N'CAPELLO Y GIANRE' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'3' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1977') WHERE concesion = 2321;

-- Fila Excel 29: CARBONETTI MARIO EDUARDO
UPDATE P SET P.nombrePanteon = N'CARBONETTI MARIO EDUARDO' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'7' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 2 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1984') WHERE concesion = 2731;

-- Fila Excel 30: CARDOZO ALICIA BLANCA
UPDATE P SET P.nombrePanteon = N'CARDOZO ALICIA BLANCA' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'2' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 5 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2014') WHERE concesion = 2369;

-- Fila Excel 31: CARDOZO DE GARAY PETRONA (c/nichos)
UPDATE P SET P.nombrePanteon = N'CARDOZO DE GARAY PETRONA (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'15' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 7 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1981') WHERE concesion = 2385;

-- Fila Excel 32: CASAMAYOR LUIS ALBERTO
UPDATE P SET P.nombrePanteon = N'CASAMAYOR LUIS ALBERTO' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'15' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 2 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1980') WHERE concesion = 2330;

-- Fila Excel 33: CASTILLO FRANCISCO Y AMALIA J.         S/PLANOS
UPDATE P SET P.nombrePanteon = N'CASTILLO FRANCISCO Y AMALIA J.         S/PLANOS' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'11' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 10 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1990') WHERE concesion = 2415;

-- Fila Excel 34: CASTRO MANUEL ANTONIO (c/cajon a la vista)
UPDATE P SET P.nombrePanteon = N'CASTRO MANUEL ANTONIO (c/cajon a la vista)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'14' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1978') WHERE concesion = 2313;

-- Fila Excel 35: CELIZ DE QUINTEROS RAMONA (c/nichos) CONTRIB FDO. CAMBAR DEU
UPDATE P SET P.nombrePanteon = N'CELIZ DE QUINTEROS RAMONA (c/nichos) CONTRIB FDO. CAMBAR DEUDOS' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'5' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1987') WHERE concesion = 2322;

-- Fila Excel 36: CERNOTTI JOSE (c/cajon a la vista)
UPDATE P SET P.nombrePanteon = N'CERNOTTI JOSE (c/cajon a la vista)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'15' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 9 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1981') WHERE concesion = 2406;

-- Fila Excel 37: CERVELLERA DE BARRIENTOS ROSA
UPDATE P SET P.nombrePanteon = N'CERVELLERA DE BARRIENTOS ROSA' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'15' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 6 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1982') WHERE concesion = 2377;

-- Fila Excel 38: CIARIMBOLI HILARIO CESAR
UPDATE P SET P.nombrePanteon = N'CIARIMBOLI HILARIO CESAR' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'3' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 2 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2019') WHERE concesion = 1255;

-- Fila Excel 39: CIPRIANI NAZARENO
UPDATE P SET P.nombrePanteon = N'CIPRIANI NAZARENO' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'19' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 4 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1994') WHERE concesion = 2357;

-- Fila Excel 40: D´ANDREA HNOS
UPDATE P SET P.nombrePanteon = N'D´ANDREA HNOS' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'10' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2010') WHERE concesion = 2310;

-- Fila Excel 41: DEL LLANO SERGIO
UPDATE P SET P.nombrePanteon = N'DEL LLANO SERGIO' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'14' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 10 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1981') WHERE concesion = 2417;

-- Fila Excel 42: DELGADO ALFREDO
UPDATE P SET P.nombrePanteon = N'DELGADO ALFREDO' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'15' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 10 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1987') WHERE concesion = 2418;

-- Fila Excel 43: DELGADO CASIMIRO ARMANDO
UPDATE P SET P.nombrePanteon = N'DELGADO CASIMIRO ARMANDO' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'15' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 8 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1981') WHERE concesion = 2396;

COMMIT TRANSACTION;
PRINT 'Lote 2 de 7 completado (filas Excel 24-43)';
GO

-- ================= LOTE 3 de 7 (filas Excel 44 a 63) =================
SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- Fila Excel 44: DONADIO NELIDA
UPDATE P SET P.nombrePanteon = N'DONADIO NELIDA' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'14' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 4 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1982') WHERE concesion = 2352;

-- Fila Excel 45: DORREGO WALDO ENRIQUE
UPDATE P SET P.nombrePanteon = N'DORREGO WALDO ENRIQUE' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'2' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 7 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1985') WHERE concesion = 2389;

-- Fila Excel 46: FASAN NORA ELENA
UPDATE P SET P.nombrePanteon = N'FASAN NORA ELENA' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'19' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 5 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2009') WHERE concesion = 2368;

-- Fila Excel 47: FAVERO GINO
UPDATE P SET P.nombrePanteon = N'FAVERO GINO' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'9' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 2 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1973') WHERE concesion = 2335;

-- Fila Excel 48: FERREYRA DE VILLAGRA ANTONIA (c/nicho)
UPDATE P SET P.nombrePanteon = N'FERREYRA DE VILLAGRA ANTONIA (c/nicho)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'13' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 5 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1981') WHERE concesion = 2707;

-- Fila Excel 49: FERREYRA NATALIA (sin construir)
UPDATE P SET P.nombrePanteon = N'FERREYRA NATALIA (sin construir)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'11' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 5 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2019') WHERE concesion = 2528;

-- Fila Excel 50: FONTANELLA Y GEACCHELLIN (con nicho)
UPDATE P SET P.nombrePanteon = N'FONTANELLA Y GEACCHELLIN (con nicho)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'1' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 6 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1975') WHERE concesion = 2371;

-- Fila Excel 51: FREZZOTTI ANA MARIA, TERESA Y BIASSI ESTER (c/nichos)
UPDATE P SET P.nombrePanteon = N'FREZZOTTI ANA MARIA, TERESA Y BIASSI ESTER (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'17' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1993') WHERE concesion = 2316;

-- Fila Excel 52: GABBARINI JULIO CESAR Y GUILLERMO JOSE (c/nichos)
UPDATE P SET P.nombrePanteon = N'GABBARINI JULIO CESAR Y GUILLERMO JOSE (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'18' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 4 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2004') WHERE concesion = 2356;

-- Fila Excel 53: GARCIA FLORINDA VDA. DE
UPDATE P SET P.nombrePanteon = N'GARCIA FLORINDA VDA. DE' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'12' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 7 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1979') WHERE concesion = 2753;

-- Fila Excel 54: GARRIDO RAMON (c/nichos)
UPDATE P SET P.nombrePanteon = N'GARRIDO RAMON (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'14' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 7 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1982') WHERE concesion = 2384;

-- Fila Excel 55: GERMANO Y NEGRO (c/nicho)
UPDATE P SET P.nombrePanteon = N'GERMANO Y NEGRO (c/nicho)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'16' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 8 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1983') WHERE concesion = 2397;

-- Fila Excel 56: GIACCHERELLO CRISTOBAL MARIO (c/nicho)
UPDATE P SET P.nombrePanteon = N'GIACCHERELLO CRISTOBAL MARIO (c/nicho)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'16' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 10 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2007') WHERE concesion = 2419;

-- Fila Excel 57: GIACCHERELLO JOSE Y OTROS
UPDATE P SET P.nombrePanteon = N'GIACCHERELLO JOSE Y OTROS' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'16' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 9 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1983') WHERE concesion = 2407;

-- Fila Excel 58: GIANNOBI HNOS
UPDATE P SET P.nombrePanteon = N'GIANNOBI HNOS' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'9' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1975') WHERE concesion = 2324;

-- Fila Excel 59: GIMENEZ PEDRO ANIBAL
UPDATE P SET P.nombrePanteon = N'GIMENEZ PEDRO ANIBAL' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'19' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 3 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2010') WHERE concesion = 2344;

-- Fila Excel 60: GIORDANO DE GUALANDRA MARIA HERMINDA
UPDATE P SET P.nombrePanteon = N'GIORDANO DE GUALANDRA MARIA HERMINDA' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'15' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 5 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1986') WHERE concesion = 2365;

-- Fila Excel 61: GIORDANO FELIPE
UPDATE P SET P.nombrePanteon = N'GIORDANO FELIPE' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'2' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 4 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1973') WHERE concesion = 2358;

-- Fila Excel 62: GIORDANO ROBERTO (C/Nichos)
UPDATE P SET P.nombrePanteon = N'GIORDANO ROBERTO (C/Nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'14' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 8 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1982') WHERE concesion = 2395;

-- Fila Excel 63: GOMEZ DE GOYOCHEA (GOYOCHEA, MORAN, GOMEZ)
UPDATE P SET P.nombrePanteon = N'GOMEZ DE GOYOCHEA (GOYOCHEA, MORAN, GOMEZ)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'11' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 9 AND P.seccionId = S.id;
-- ADVERTENCIA: sin numero de concesion en el Excel, no se pudo actualizar Concesiones para esta fila

COMMIT TRANSACTION;
PRINT 'Lote 3 de 7 completado (filas Excel 44-63)';
GO

-- ================= LOTE 4 de 7 (filas Excel 64 a 83) =================
SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- Fila Excel 64: GURUCETA HECTOR CELSO Y CAMINOS ELVIRA (c/nichos)
UPDATE P SET P.nombrePanteon = N'GURUCETA HECTOR CELSO Y CAMINOS ELVIRA (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'16' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 3 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1990') WHERE concesion = 2341;

-- Fila Excel 65: GUTIERREZ LEA (c/cajon a la vista)
UPDATE P SET P.nombrePanteon = N'GUTIERREZ LEA (c/cajon a la vista)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'10' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 4 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1976') WHERE concesion = 2349;

-- Fila Excel 66: IBARRA MARIA LUISA DEL VALLE
UPDATE P SET P.nombrePanteon = N'IBARRA MARIA LUISA DEL VALLE' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'16' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 5 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2023') WHERE concesion = 2948;

-- Fila Excel 67: LAURET MARCELINO (c/cajon a la vista)
UPDATE P SET P.nombrePanteon = N'LAURET MARCELINO (c/cajon a la vista)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'2' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 8 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1976') WHERE concesion = 2400;

-- Fila Excel 68: LONDERO JOSE (c/cajon a la vista)
UPDATE P SET P.nombrePanteon = N'LONDERO JOSE (c/cajon a la vista)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'9' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 10 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1974') WHERE concesion = 2421;

-- Fila Excel 69: LOPEZ PEDRO Y MORALES
UPDATE P SET P.nombrePanteon = N'LOPEZ PEDRO Y MORALES' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'12' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 2 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1977') WHERE concesion = 2328;

-- Fila Excel 70: LOPEZ SANCHEZ MARIA DEL CARMEN (c/nichos)
UPDATE P SET P.nombrePanteon = N'LOPEZ SANCHEZ MARIA DEL CARMEN (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'17' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 4 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2011') WHERE concesion = 2355;

-- Fila Excel 71: LORENZONI ELIAS (c/cajon a la vista)
UPDATE P SET P.nombrePanteon = N'LORENZONI ELIAS (c/cajon a la vista)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'9' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 3 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1977') WHERE concesion = 2347;

-- Fila Excel 72: LUCHETTI DE AMAYA (c/nichos)
UPDATE P SET P.nombrePanteon = N'LUCHETTI DE AMAYA (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'17' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 7 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1999') WHERE concesion = 2387;

-- Fila Excel 73: LUCHETTI LUIS OSCAR (c/nichos)
UPDATE P SET P.nombrePanteon = N'LUCHETTI LUIS OSCAR (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'18' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 6 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2000') WHERE concesion = 2380;

-- Fila Excel 74: LUDUEÑA DE BAZAN MARIA ZUNILDA (c/nicho) CONTRIB FDO, CAMBAR
UPDATE P SET P.nombrePanteon = N'LUDUEÑA DE BAZAN MARIA ZUNILDA (c/nicho) CONTRIB FDO, CAMBAR DEUDOS' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'16' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 7 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1985') WHERE concesion = 2386;

-- Fila Excel 75: LUDUEÑA EDUARDO Y ACEVEDO JUAN
UPDATE P SET P.nombrePanteon = N'LUDUEÑA EDUARDO Y ACEVEDO JUAN' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'17' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 2 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1993') WHERE concesion = 2331;

-- Fila Excel 76: LUQUE DE MEDRANO MARIA CORINA (c/nichos)
UPDATE P SET P.nombrePanteon = N'LUQUE DE MEDRANO MARIA CORINA (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'16' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 6 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1983') WHERE concesion = 2378;

-- Fila Excel 77: MAESTRE JUAN CARLOS (JOSE Y RAFAEL)
UPDATE P SET P.nombrePanteon = N'MAESTRE JUAN CARLOS (JOSE Y RAFAEL)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'21' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 9 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1995') WHERE concesion = 2411;

-- Fila Excel 78: MAGRO JOSE   (c/nicho)
UPDATE P SET P.nombrePanteon = N'MAGRO JOSE   (c/nicho)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'1' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 5 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1978') WHERE concesion = 2361;

-- Fila Excel 79: MAINARDI LUIS Y ZAMAI
UPDATE P SET P.nombrePanteon = N'MAINARDI LUIS Y ZAMAI' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'14' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 6 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1983') WHERE concesion = 2376;

-- Fila Excel 80: MARCIAL JUAN
UPDATE P SET P.nombrePanteon = N'MARCIAL JUAN' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'10' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 9 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1974') WHERE concesion = 2402;

-- Fila Excel 81: MARTIN CARLOS A. Y LUCHETTI RAUL H. (c/nichos)
UPDATE P SET P.nombrePanteon = N'MARTIN CARLOS A. Y LUCHETTI RAUL H. (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'15' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 3 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1982') WHERE concesion = 2536;

-- Fila Excel 82: MARTIN EDUARDO (C/cajon a la vista)
UPDATE P SET P.nombrePanteon = N'MARTIN EDUARDO (C/cajon a la vista)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'2' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 10 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1975') WHERE concesion = 2420;

-- Fila Excel 83: MARTIN JOSE MARIA (C/cajon a la vista)
UPDATE P SET P.nombrePanteon = N'MARTIN JOSE MARIA (C/cajon a la vista)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'2' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 9 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1975') WHERE concesion = 2410;

COMMIT TRANSACTION;
PRINT 'Lote 4 de 7 completado (filas Excel 64-83)';
GO

-- ================= LOTE 5 de 7 (filas Excel 84 a 103) =================
SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- Fila Excel 84: MARTINEZ PEDRO Y ERNESTO
UPDATE P SET P.nombrePanteon = N'MARTINEZ PEDRO Y ERNESTO' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'13' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2006') WHERE concesion = 2312;

-- Fila Excel 85: MATTOS ALFREDO Y HNOS.
UPDATE P SET P.nombrePanteon = N'MATTOS ALFREDO Y HNOS.' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'20' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 4 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2011') WHERE concesion = 2359;

-- Fila Excel 86: MEANA RAFAEL
UPDATE P SET P.nombrePanteon = N'MEANA RAFAEL' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'1' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 8 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1975') WHERE concesion = 2391;

-- Fila Excel 87: MENOTTA IVO (c/nichos)
UPDATE P SET P.nombrePanteon = N'MENOTTA IVO (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'10' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 5 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1988') WHERE concesion = 2362;

-- Fila Excel 88: MIRA GINES
UPDATE P SET P.nombrePanteon = N'MIRA GINES' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'15' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1982') WHERE concesion = 2314;

-- Fila Excel 89: MIRA JOSEFA Y MARTIN
UPDATE P SET P.nombrePanteon = N'MIRA JOSEFA Y MARTIN' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'15' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 4 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1982') WHERE concesion = 2353;

-- Fila Excel 90: MIRAGLIA HUMBERTO E HIJOS
UPDATE P SET P.nombrePanteon = N'MIRAGLIA HUMBERTO E HIJOS' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'22' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 9 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2005') WHERE concesion = 2412;

-- Fila Excel 91: MOJICA HERMINDA DE MATTO (c/nichos)
UPDATE P SET P.nombrePanteon = N'MOJICA HERMINDA DE MATTO (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'2' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 2 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1972') WHERE concesion = 2333;

-- Fila Excel 92: MONSO EDUARDO
UPDATE P SET P.nombrePanteon = N'MONSO EDUARDO' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'13' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 4 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1980') WHERE concesion = 2351;

-- Fila Excel 93: MONTEIRO RIBEIRO JOSE Y HNOS. (C/cajon a la vista)
UPDATE P SET P.nombrePanteon = N'MONTEIRO RIBEIRO JOSE Y HNOS. (C/cajon a la vista)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'1' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 7 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1976') WHERE concesion = 2382;

-- Fila Excel 94: MONTERUBIANESI PIA
UPDATE P SET P.nombrePanteon = N'MONTERUBIANESI PIA' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'20' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2007') WHERE concesion = 2319;

-- Fila Excel 95: MORILLO MARCELO FABIAN
UPDATE P SET P.nombrePanteon = N'MORILLO MARCELO FABIAN' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'17' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 9 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2007') WHERE concesion = 2408;

-- Fila Excel 96: MORILLO MARIA Y MIRAS EMILIA (c/nichos)
UPDATE P SET P.nombrePanteon = N'MORILLO MARIA Y MIRAS EMILIA (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'18' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 2 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1993') WHERE concesion = 2332;

-- Fila Excel 97: MOYANO CANDIDA Y CARRIZO NORMA DEL VALLE
UPDATE P SET P.nombrePanteon = N'MOYANO CANDIDA Y CARRIZO NORMA DEL VALLE' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'18' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 9 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2000') WHERE concesion = 2409;

-- Fila Excel 98: MOYANO RAMON OSCAR
UPDATE P SET P.nombrePanteon = N'MOYANO RAMON OSCAR' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'16' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1987') WHERE concesion = 2315;

-- Fila Excel 99: NAVARRO DELFOR
UPDATE P SET P.nombrePanteon = N'NAVARRO DELFOR' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'13' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 2 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1978') WHERE concesion = 2329;

-- Fila Excel 100: OLIVO BIASI                           SIN EDIFICAR
UPDATE P SET P.nombrePanteon = N'OLIVO BIASI                           SIN EDIFICAR' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'21' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
-- ADVERTENCIA: sin numero de concesion en el Excel, no se pudo actualizar Concesiones para esta fila

-- Fila Excel 101: OLMOS NOE Y OTROS (c/nichos)
UPDATE P SET P.nombrePanteon = N'OLMOS NOE Y OTROS (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'9' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 6 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1974') WHERE concesion = 2381;

-- Fila Excel 102: OPPERMAN Y ROVERO
UPDATE P SET P.nombrePanteon = N'OPPERMAN Y ROVERO' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'13' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 8 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1980') WHERE concesion = 2394;

-- Fila Excel 103: OSELLA ROBERTO A. Y GARCIA OLGA
UPDATE P SET P.nombrePanteon = N'OSELLA ROBERTO A. Y GARCIA OLGA' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'1' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 2 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1990') WHERE concesion = 2325;

COMMIT TRANSACTION;
PRINT 'Lote 5 de 7 completado (filas Excel 84-103)';
GO

-- ================= LOTE 6 de 7 (filas Excel 104 a 123) =================
SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- Fila Excel 104: PEDANO DE CORBACHO ROSA (c/nichos)
UPDATE P SET P.nombrePanteon = N'PEDANO DE CORBACHO ROSA (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'19' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2005') WHERE concesion = 2318;

-- Fila Excel 105: PEDERNERA CRISPIN (c/nichos)
UPDATE P SET P.nombrePanteon = N'PEDERNERA CRISPIN (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'5' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 2 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1978') WHERE concesion = 2334;

-- Fila Excel 106: PEÑALOZA FELIPE
UPDATE P SET P.nombrePanteon = N'PEÑALOZA FELIPE' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'10' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 10 AND P.seccionId = S.id;
-- ADVERTENCIA: sin numero de concesion en el Excel, no se pudo actualizar Concesiones para esta fila

-- Fila Excel 107: PEREZ ANGELA Y PETRIA (PREGUNTAR AÑO DE ADQUISICION)
UPDATE P SET P.nombrePanteon = N'PEREZ ANGELA Y PETRIA (PREGUNTAR AÑO DE ADQUISICION)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'9' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 8 AND P.seccionId = S.id;
-- ADVERTENCIA: sin anio de adquisicion en el Excel (revisar 'PEREZ ANGELA Y PETRIA (PREGUNTAR AÑO DE ADQUISICION)'), no se actualizo informacionAdicional

-- Fila Excel 108: PEREZ DOMINGO
UPDATE P SET P.nombrePanteon = N'PEREZ DOMINGO' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'13' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 9 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1981') WHERE concesion = 2404;

-- Fila Excel 109: PINEDA BERTA ISABEL (c/nichos)
UPDATE P SET P.nombrePanteon = N'PINEDA BERTA ISABEL (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'1' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 4 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2007') WHERE concesion = 2348;

-- Fila Excel 110: PIOVANO MIGUEL Y JORGE       SIN EDIFICAR S/PLANOS
UPDATE P SET P.nombrePanteon = N'PIOVANO MIGUEL Y JORGE       SIN EDIFICAR S/PLANOS' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'11' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 4 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2007') WHERE concesion = 2350;

-- Fila Excel 111: PONSELLA STELLA MARIS
UPDATE P SET P.nombrePanteon = N'PONSELLA STELLA MARIS' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'12' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 5 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2014') WHERE concesion = 2363;

-- Fila Excel 112: PORCHETTI Y CONTRERAS     (c/nicho)
UPDATE P SET P.nombrePanteon = N'PORCHETTI Y CONTRERAS     (c/nicho)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'1' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1975') WHERE concesion = 2309;

-- Fila Excel 113: PUGLIE
UPDATE P SET P.nombrePanteon = N'PUGLIE' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'12' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 3 AND P.seccionId = S.id;
UPDATE P SET P.nombrePanteon = N'PUGLIE' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'12' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 4 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1979') WHERE concesion = 2338;

-- Fila Excel 114: QUARIN ERNESTO ARMANDO (C/Nichos) CONTRIB FDO CAMBAR DEUDOS
UPDATE P SET P.nombrePanteon = N'QUARIN ERNESTO ARMANDO (C/Nichos) CONTRIB FDO CAMBAR DEUDOS' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'11' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 3 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1990') WHERE concesion = 2337;

-- Fila Excel 115: QUERZOLA ETELVINA DORA (Retirar contrato)
UPDATE P SET P.nombrePanteon = N'QUERZOLA ETELVINA DORA (Retirar contrato)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'10' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 3 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2012') WHERE concesion = 2336;

-- Fila Excel 116: QUINTEROS SARA SIMONA (c/nichos)
UPDATE P SET P.nombrePanteon = N'QUINTEROS SARA SIMONA (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'18' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 3 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2001') WHERE concesion = 2343;

-- Fila Excel 117: RECH Y RUFFINI
UPDATE P SET P.nombrePanteon = N'RECH Y RUFFINI' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'12' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 8 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1981') WHERE concesion = 2721;

-- Fila Excel 118: RICCO GUMERCINDO Y OTROS (c/nicho)
UPDATE P SET P.nombrePanteon = N'RICCO GUMERCINDO Y OTROS (c/nicho)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'14' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 5 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1982') WHERE concesion = 2364;

-- Fila Excel 119: RINALDI Y MOLLICA
UPDATE P SET P.nombrePanteon = N'RINALDI Y MOLLICA' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'12' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 6 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1980') WHERE concesion = 2374;

-- Fila Excel 120: ROCH ELODIA DEL CARMEN (c/nichos)
UPDATE P SET P.nombrePanteon = N'ROCH ELODIA DEL CARMEN (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'17' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 8 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1996') WHERE concesion = 2398;

-- Fila Excel 121: ROMERO DE BORDI ALIDA (c/nichos)
UPDATE P SET P.nombrePanteon = N'ROMERO DE BORDI ALIDA (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'22' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1994') WHERE concesion = 2320;

-- Fila Excel 122: ROSTIROLLA DOLA
UPDATE P SET P.nombrePanteon = N'ROSTIROLLA DOLA' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'9' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 7 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1981') WHERE concesion = 2390;

-- Fila Excel 123: ROSTIROLLA HECTOR RAUL
UPDATE P SET P.nombrePanteon = N'ROSTIROLLA HECTOR RAUL' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'18' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 7 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1996') WHERE concesion = 2388;

COMMIT TRANSACTION;
PRINT 'Lote 6 de 7 completado (filas Excel 104-123)';
GO

-- ================= LOTE 7 de 7 (filas Excel 124 a 137) =================
SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- Fila Excel 124: RUFFINI MARIO Y ELIO (c/cajon a la vista)
UPDATE P SET P.nombrePanteon = N'RUFFINI MARIO Y ELIO (c/cajon a la vista)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'9' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 4 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1974') WHERE concesion = 2360;

-- Fila Excel 125: RUGGERI HNOS. (C/cajon a la vista)
UPDATE P SET P.nombrePanteon = N'RUGGERI HNOS. (C/cajon a la vista)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'2' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 3 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1973') WHERE concesion = 2345;

-- Fila Excel 126: RUIZ Y MARTINEZ        (c/nicho)
UPDATE P SET P.nombrePanteon = N'RUIZ Y MARTINEZ        (c/nicho)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'9' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 5 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1989') WHERE concesion = 2370;

-- Fila Excel 127: SECULINI MARCELO Y ADRIAN
UPDATE P SET P.nombrePanteon = N'SECULINI MARCELO Y ADRIAN' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'16' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 2 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2023') WHERE concesion = 2967;

-- Fila Excel 128: SOTO JORGE EDUARDO Y VICTOR HUGO (c/nichos)
UPDATE P SET P.nombrePanteon = N'SOTO JORGE EDUARDO Y VICTOR HUGO (c/nichos)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'17' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 6 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1996') WHERE concesion = 2379;

-- Fila Excel 129: TAMBURINI LUIS Y OTROS
UPDATE P SET P.nombrePanteon = N'TAMBURINI LUIS Y OTROS' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'9' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 9 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1976') WHERE concesion = 2413;

-- Fila Excel 130: TRASOBARES HNOS.  (c/nicho)
UPDATE P SET P.nombrePanteon = N'TRASOBARES HNOS.  (c/nicho)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'12' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 9 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1981') WHERE concesion = 2403;

-- Fila Excel 131: TULIAN DE SACCO BLANCA
UPDATE P SET P.nombrePanteon = N'TULIAN DE SACCO BLANCA' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'13' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 3 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1981') WHERE concesion = 2339;

-- Fila Excel 132: VACCARO JOSE Y OTROS
UPDATE P SET P.nombrePanteon = N'VACCARO JOSE Y OTROS' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'12' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 1 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1976') WHERE concesion = 2311;

-- Fila Excel 133: VAZQUEZ MIGUEL ANGEL
UPDATE P SET P.nombrePanteon = N'VAZQUEZ MIGUEL ANGEL' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'22' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 8 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2021') WHERE concesion = 2875;

-- Fila Excel 134: VEGA CECILIA Y OTROS (C/cajon a la vista)
UPDATE P SET P.nombrePanteon = N'VEGA CECILIA Y OTROS (C/cajon a la vista)' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'1' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 10 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1975') WHERE concesion = 2414;

-- Fila Excel 135: VELAZQUEZ MARTIN
UPDATE P SET P.nombrePanteon = N'VELAZQUEZ MARTIN' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'11' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 2 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1987') WHERE concesion = 2327;

-- Fila Excel 136: ZALLOCCO ANALIA GISELA
UPDATE P SET P.nombrePanteon = N'ZALLOCCO ANALIA GISELA' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'19' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 2 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 2021') WHERE concesion = 2833;

-- Fila Excel 137: ZITELLI LUIS
UPDATE P SET P.nombrePanteon = N'ZITELLI LUIS' FROM Parcelas P INNER JOIN Secciones S ON S.nombre = N'1' AND S.tipoParcelaId = 3 WHERE P.nroParcela = 9 AND P.seccionId = S.id;
UPDATE Concesiones SET informacionAdicional = CONCAT(ISNULL(informacionAdicional, N''), CASE WHEN informacionAdicional IS NULL OR informacionAdicional = N'' THEN N'' ELSE N' | ' END, N'Año de adquisición: 1976') WHERE concesion = 2401;

COMMIT TRANSACTION;
PRINT 'Lote 7 de 7 completado (filas Excel 124-137)';
GO

