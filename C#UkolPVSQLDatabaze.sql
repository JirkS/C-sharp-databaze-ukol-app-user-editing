use syrovatko;

create table zakaznik(
id int primary key identity(1,1),
jmeno varchar(20) not null,
email varchar(40) unique check(email like '%@%') 
);

create table objednavka(
id int primary key identity(1,1),
zak_id int not null foreign key references zakaznik(id),
cislo_obj int unique not null check(cislo_obj > 0),
datum date not null,
cena_obj int not null check(cena_obj > 0)
);

create table vyrobce(
id int primary key identity(1,1),
nazev varchar(20) not null,
email varchar(40) unique check(email like '%@%'),
overeny int not null check(overeny in (1,0))
);

create table vyrobek(
id int primary key identity(1,1),
vyrobce_id int not null foreign key references vyrobce(id),
typ varchar(20) not null check(typ in ('obchodni', 'reklamni', 'provozni')),
nazev varchar(20) not null,
cena_ks int not null check(cena_ks > 0)
);

create table polozka(
id int primary key identity(1,1),
obj_id int not null foreign key references objednavka(id),
vyrobek_id int not null foreign key references vyrobek(id),
pocet_ks int not null check(pocet_ks > 0),
cena_polozky float not null check(cena_polozky > 0)	
);