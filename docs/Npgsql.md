Npgsql
==========================================================================


```
postgres=# SELECT version();
                                 version
-------------------------------------------------------------------------
 PostgreSQL 17.2 on x86_64-windows, compiled by msvc-19.42.34435, 64-bit
(1 row)
```


```
SELECT ns.nspname, t.oid, t.typname, t.typtype, t.typnotnull, t.elemtypoid
FROM (
    -- Arrays have typtype=b - this subquery identifies them by their typreceive and converts their typtype to a
    -- We first do this for the type (innerest-most subquery), and then for its element type
    -- This also returns the array element, range subtype and domain base type as elemtypoid
    SELECT
        typ.oid, typ.typnamespace, typ.typname, typ.typtype, typ.typrelid, typ.typnotnull, typ.relkind,
        elemtyp.oid AS elemtypoid, elemtyp.typname AS elemtypname, elemcls.relkind AS elemrelkind,
        CASE WHEN elemproc.proname='array_recv' THEN 'a' ELSE elemtyp.typtype END AS elemtyptype
        , typ.typcategory
    FROM (
        SELECT typ.oid, typnamespace, typname, typrelid, typnotnull, relkind, typelem AS elemoid,
            CASE WHEN proc.proname='array_recv' THEN 'a' ELSE typ.typtype END AS typtype,
            CASE
                WHEN proc.proname='array_recv' THEN typ.typelem
                WHEN typ.typtype='r' THEN rngsubtype

                WHEN typ.typtype='d' THEN typ.typbasetype
            END AS elemtypoid
            , typ.typcategory
        FROM pg_type AS typ
        LEFT JOIN pg_class AS cls ON (cls.oid = typ.typrelid)
        LEFT JOIN pg_proc AS proc ON proc.oid = typ.typreceive
        LEFT JOIN pg_range ON (pg_range.rngtypid = typ.oid)
    ) AS typ
    LEFT JOIN pg_type AS elemtyp ON elemtyp.oid = elemtypoid
    LEFT JOIN pg_class AS elemcls ON (elemcls.oid = elemtyp.typrelid)
    LEFT JOIN pg_proc AS elemproc ON elemproc.oid = elemtyp.typreceive
) AS t
JOIN pg_namespace AS ns ON (ns.oid = typnamespace)
WHERE
    (
    typtype IN ('b', 'r', 'm', 'e', 'd') OR -- Base, range, multirange, enum, domain
    (typtype = 'c' AND relkind='c') OR -- User-defined free-standing composites (not table composites) by default
    (typtype = 'p' AND typname IN ('record', 'void', 'unknown')) OR -- Some special supported pseudo-types
    (typtype = 'a' AND (  -- Array of...
        elemtyptype IN ('b', 'r', 'm', 'e', 'd') OR -- Array of base, range, multirange, enum, domain
        (elemtyptype = 'p' AND elemtypname IN ('record', 'void')) OR -- Arrays of special supported pseudo-types
        (elemtyptype = 'c' AND elemrelkind='c') -- Array of user-defined free-standing composites (not table composites) by default
    )))
ORDER BY CASE
       WHEN typtype IN ('b', 'e', 'p') THEN 0           -- First base types, enums, pseudo-types
       WHEN typtype = 'c' THEN 1                        -- Composites after (fields loaded later in 2nd pass)
       WHEN typtype = 'r' THEN 2                        -- Ranges after
       WHEN typtype = 'm' THEN 3                        -- Multiranges after
       WHEN typtype = 'd' AND elemtyptype <> 'a' THEN 4 -- Domains over non-arrays after
       WHEN typtype = 'a' THEN 5                        -- Arrays after
       WHEN typtype = 'd' AND elemtyptype = 'a' THEN 6  -- Domains over arrays last
END;

      nspname       |  oid  |           typname            | typtype | typnotnull | elemtypoid
--------------------+-------+------------------------------+---------+------------+------------
 pg_catalog         |   700 | float4                       | b       | f          |
 pg_catalog         |    23 | int4                         | b       | f          |
 pg_catalog         |    24 | regproc                      | b       | f          |
 pg_catalog         |    25 | text                         | b       | f          |
 pg_catalog         |    26 | oid                          | b       | f          |
 pg_catalog         |    27 | tid                          | b       | f          |
 pg_catalog         |    28 | xid                          | b       | f          |
 pg_catalog         |    29 | cid                          | b       | f          |
 pg_catalog         |    30 | oidvector                    | b       | f          |
 pg_catalog         |   114 | json                         | b       | f          |
 pg_catalog         |   142 | xml                          | b       | f          |
 pg_catalog         |   194 | pg_node_tree                 | b       | f          |
 pg_catalog         |  3361 | pg_ndistinct                 | b       | f          |
 pg_catalog         |  3402 | pg_dependencies              | b       | f          |
 pg_catalog         |  5017 | pg_mcv_list                  | b       | f          |
 pg_catalog         |  5069 | xid8                         | b       | f          |
 pg_catalog         |   600 | point                        | b       | f          |
 pg_catalog         |   601 | lseg                         | b       | f          |
 pg_catalog         |   602 | path                         | b       | f          |
 pg_catalog         |   603 | box                          | b       | f          |
 pg_catalog         |   604 | polygon                      | b       | f          |
 pg_catalog         |   628 | line                         | b       | f          |
 pg_catalog         |    22 | int2vector                   | b       | f          |
 pg_catalog         |   701 | float8                       | b       | f          |
 pg_catalog         |   705 | unknown                      | p       | f          |
 pg_catalog         |   718 | circle                       | b       | f          |
 pg_catalog         |   790 | money                        | b       | f          |
 pg_catalog         |   829 | macaddr                      | b       | f          |
 pg_catalog         |   869 | inet                         | b       | f          |
 pg_catalog         |   650 | cidr                         | b       | f          |
 pg_catalog         |   774 | macaddr8                     | b       | f          |
 pg_catalog         |  1033 | aclitem                      | b       | f          |
 pg_catalog         |  1042 | bpchar                       | b       | f          |
 pg_catalog         |  1043 | varchar                      | b       | f          |
 pg_catalog         |  1082 | date                         | b       | f          |
 pg_catalog         |  1083 | time                         | b       | f          |
 pg_catalog         |  1114 | timestamp                    | b       | f          |
 pg_catalog         |  1184 | timestamptz                  | b       | f          |
 pg_catalog         |  1186 | interval                     | b       | f          |
 pg_catalog         |  1266 | timetz                       | b       | f          |
 pg_catalog         |  1560 | bit                          | b       | f          |
 pg_catalog         |  1562 | varbit                       | b       | f          |
 pg_catalog         |  1700 | numeric                      | b       | f          |
 pg_catalog         |  1790 | refcursor                    | b       | f          |
 pg_catalog         |  2202 | regprocedure                 | b       | f          |
 pg_catalog         |  2203 | regoper                      | b       | f          |
 pg_catalog         |  2204 | regoperator                  | b       | f          |
 pg_catalog         |  2205 | regclass                     | b       | f          |
 pg_catalog         |  4191 | regcollation                 | b       | f          |
 pg_catalog         |  2206 | regtype                      | b       | f          |
 pg_catalog         |  4096 | regrole                      | b       | f          |
 pg_catalog         |  4089 | regnamespace                 | b       | f          |
 pg_catalog         |  2950 | uuid                         | b       | f          |
 pg_catalog         |  3220 | pg_lsn                       | b       | f          |
 pg_catalog         |  3614 | tsvector                     | b       | f          |
 pg_catalog         |  3642 | gtsvector                    | b       | f          |
 pg_catalog         |  3615 | tsquery                      | b       | f          |
 pg_catalog         |  3734 | regconfig                    | b       | f          |
 pg_catalog         |  3769 | regdictionary                | b       | f          |
 pg_catalog         |  3802 | jsonb                        | b       | f          |
 pg_catalog         |  4072 | jsonpath                     | b       | f          |
 pg_catalog         |  2970 | txid_snapshot                | b       | f          |
 pg_catalog         |  5038 | pg_snapshot                  | b       | f          |
 pg_catalog         |  2249 | record                       | p       | f          |
 pg_catalog         |    16 | bool                         | b       | f          |
 pg_catalog         |    17 | bytea                        | b       | f          |
 pg_catalog         |  2278 | void                         | p       | f          |
 pg_catalog         |  4600 | pg_brin_bloom_summary        | b       | f          |
 pg_catalog         |  4601 | pg_brin_minmax_multi_summary | b       | f          |
 pg_catalog         |    18 | char                         | b       | f          |
 pg_catalog         |    19 | name                         | b       | f          |
 pg_catalog         |    20 | int8                         | b       | f          |
 pg_catalog         |    21 | int2                         | b       | f          |
 pg_catalog         |  3908 | tsrange                      | r       | f          |       1114
 pg_catalog         |  3904 | int4range                    | r       | f          |         23
 pg_catalog         |  3906 | numrange                     | r       | f          |       1700
 pg_catalog         |  3912 | daterange                    | r       | f          |       1082
 pg_catalog         |  3926 | int8range                    | r       | f          |         20
 pg_catalog         |  3910 | tstzrange                    | r       | f          |       1184
 pg_catalog         |  4534 | tstzmultirange               | m       | f          |
 pg_catalog         |  4535 | datemultirange               | m       | f          |
 pg_catalog         |  4536 | int8multirange               | m       | f          |
 pg_catalog         |  4451 | int4multirange               | m       | f          |
 pg_catalog         |  4532 | nummultirange                | m       | f          |
 pg_catalog         |  4533 | tsmultirange                 | m       | f          |
 information_schema | 14637 | character_data               | d       | f          |       1043
 information_schema | 14634 | cardinal_number              | d       | f          |         23
 information_schema | 14647 | yes_or_no                    | d       | f          |       1043
 information_schema | 14645 | time_stamp                   | d       | f          |       1184
 information_schema | 14639 | sql_identifier               | d       | f          |         19
 information_schema | 14646 | _yes_or_no                   | a       | f          |      14647
 pg_catalog         |  2287 | _record                      | a       | f          |       2249
 pg_catalog         |  1000 | _bool                        | a       | f          |         16
 pg_catalog         |  1001 | _bytea                       | a       | f          |         17
 pg_catalog         |  1002 | _char                        | a       | f          |         18
 pg_catalog         |  1003 | _name                        | a       | f          |         19
 pg_catalog         |  1016 | _int8                        | a       | f          |         20
 pg_catalog         |  1005 | _int2                        | a       | f          |         21
 pg_catalog         |  1006 | _int2vector                  | a       | f          |         22
 pg_catalog         |  1007 | _int4                        | a       | f          |         23
 pg_catalog         |  1008 | _regproc                     | a       | f          |         24
 pg_catalog         |  1009 | _text                        | a       | f          |         25
 pg_catalog         |  1028 | _oid                         | a       | f          |         26
 pg_catalog         |  1010 | _tid                         | a       | f          |         27
 pg_catalog         |  1011 | _xid                         | a       | f          |         28
 pg_catalog         |  1012 | _cid                         | a       | f          |         29
 pg_catalog         |  1013 | _oidvector                   | a       | f          |         30
 pg_catalog         |   199 | _json                        | a       | f          |        114
 pg_catalog         |   143 | _xml                         | a       | f          |        142
 pg_catalog         |   271 | _xid8                        | a       | f          |       5069
 pg_catalog         |  1017 | _point                       | a       | f          |        600
 pg_catalog         |  1018 | _lseg                        | a       | f          |        601
 pg_catalog         |  1019 | _path                        | a       | f          |        602
 pg_catalog         |  1020 | _box                         | a       | f          |        603
 pg_catalog         |  1027 | _polygon                     | a       | f          |        604
 pg_catalog         |   629 | _line                        | a       | f          |        628
 pg_catalog         |  1021 | _float4                      | a       | f          |        700
 pg_catalog         |  1022 | _float8                      | a       | f          |        701
 pg_catalog         |   719 | _circle                      | a       | f          |        718
 pg_catalog         |   791 | _money                       | a       | f          |        790
 pg_catalog         |  1040 | _macaddr                     | a       | f          |        829
 pg_catalog         |  1041 | _inet                        | a       | f          |        869
 pg_catalog         |   651 | _cidr                        | a       | f          |        650
 pg_catalog         |   775 | _macaddr8                    | a       | f          |        774
 pg_catalog         |  1034 | _aclitem                     | a       | f          |       1033
 pg_catalog         |  1014 | _bpchar                      | a       | f          |       1042
 pg_catalog         |  1015 | _varchar                     | a       | f          |       1043
 pg_catalog         |  1182 | _date                        | a       | f          |       1082
 pg_catalog         |  1183 | _time                        | a       | f          |       1083
 pg_catalog         |  1115 | _timestamp                   | a       | f          |       1114
 pg_catalog         |  1185 | _timestamptz                 | a       | f          |       1184
 pg_catalog         |  1187 | _interval                    | a       | f          |       1186
 pg_catalog         |  1270 | _timetz                      | a       | f          |       1266
 pg_catalog         |  1561 | _bit                         | a       | f          |       1560
 pg_catalog         |  1563 | _varbit                      | a       | f          |       1562
 pg_catalog         |  1231 | _numeric                     | a       | f          |       1700
 pg_catalog         |  2201 | _refcursor                   | a       | f          |       1790
 pg_catalog         |  2207 | _regprocedure                | a       | f          |       2202
 pg_catalog         |  2208 | _regoper                     | a       | f          |       2203
 pg_catalog         |  2209 | _regoperator                 | a       | f          |       2204
 pg_catalog         |  2210 | _regclass                    | a       | f          |       2205
 pg_catalog         |  4192 | _regcollation                | a       | f          |       4191
 pg_catalog         |  2211 | _regtype                     | a       | f          |       2206
 pg_catalog         |  4097 | _regrole                     | a       | f          |       4096
 pg_catalog         |  4090 | _regnamespace                | a       | f          |       4089
 pg_catalog         |  2951 | _uuid                        | a       | f          |       2950
 pg_catalog         |  3221 | _pg_lsn                      | a       | f          |       3220
 pg_catalog         |  3643 | _tsvector                    | a       | f          |       3614
 pg_catalog         |  3644 | _gtsvector                   | a       | f          |       3642
 pg_catalog         |  3645 | _tsquery                     | a       | f          |       3615
 pg_catalog         |  3735 | _regconfig                   | a       | f          |       3734
 pg_catalog         |  3770 | _regdictionary               | a       | f          |       3769
 pg_catalog         |  3807 | _jsonb                       | a       | f          |       3802
 pg_catalog         |  4073 | _jsonpath                    | a       | f          |       4072
 pg_catalog         |  2949 | _txid_snapshot               | a       | f          |       2970
 pg_catalog         |  5039 | _pg_snapshot                 | a       | f          |       5038
 pg_catalog         |  3905 | _int4range                   | a       | f          |       3904
 pg_catalog         |  3907 | _numrange                    | a       | f          |       3906
 pg_catalog         |  3909 | _tsrange                     | a       | f          |       3908
 pg_catalog         |  3911 | _tstzrange                   | a       | f          |       3910
 pg_catalog         |  3913 | _daterange                   | a       | f          |       3912
 pg_catalog         |  3927 | _int8range                   | a       | f          |       3926
 pg_catalog         |  6150 | _int4multirange              | a       | f          |       4451
 pg_catalog         |  6151 | _nummultirange               | a       | f          |       4532
 pg_catalog         |  6152 | _tsmultirange                | a       | f          |       4533
 pg_catalog         |  6153 | _tstzmultirange              | a       | f          |       4534
 pg_catalog         |  6155 | _datemultirange              | a       | f          |       4535
 pg_catalog         |  6157 | _int8multirange              | a       | f          |       4536
 information_schema | 14633 | _cardinal_number             | a       | f          |      14634
 information_schema | 14636 | _character_data              | a       | f          |      14637
 information_schema | 14638 | _sql_identifier              | a       | f          |      14639
 information_schema | 14644 | _time_stamp                  | a       | f          |      14645
```


```
-- Load field definitions for (free-standing) composite types
SELECT typ.oid, att.attname, att.atttypid
FROM pg_type AS typ
JOIN pg_namespace AS ns ON (ns.oid = typ.typnamespace)
JOIN pg_class AS cls ON (cls.oid = typ.typrelid)
JOIN pg_attribute AS att ON (att.attrelid = typ.typrelid)
WHERE
  (typ.typtype = 'c' AND cls.relkind='c') AND

  attnum > 0 AND     -- Don't load system attributes
  NOT attisdropped
ORDER BY typ.oid, att.attnum;

 oid | attname | atttypid
-----+---------+----------
(0 rows)
```


```
-- Load enum fields
SELECT pg_type.oid, enumlabel
FROM pg_enum
JOIN pg_type ON pg_type.oid=enumtypid
ORDER BY oid, enumsortorder;

 oid | enumlabel
-----+-----------
(0 rows)
```


