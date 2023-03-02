## App user databse

aplikace se spouští ve Visual Studiu 2022

po načtení se Vám zobrazí nabídka (horní vodorovné menu) s 5 taby (na každém jiná část)

po pravé straně se nachází základní tlačítka pro přidání, smazání a upravu objektů do/z/v databázi a ancteni dat z csv filu

volbu práce s různými objekty si volíte dle zvoleného tabu (popřípadě políčkem mezi tlačítky po pravé straně)

v projektu je vytvořen konfigurační soubor pro snadné připojení do databáze

součástí projektu je i .sql file s databazí a obrazek propojení tabulek.

!před zapnutím zkontrolujte vše v souboru App.config

### Tlačítka:

PŘIDAT:

- do políček, ve zvoleném tabu, zadáte validní data a poté kliknete na tlačítko PŘIDAT -> pokud vše proběhlo úspěšně, políčka se vynulují, jinak Vám program zobrazí chybu
- poté je objekt přidán do databáze

SMAZAT:

- mezi tlačítky SMAZAT a UPRAVIT se nachází políčko, které pracuje s oběmi tlačítky
- v každém tabu je po jeho levé straně napsáné, co vyžaduje
- pokud chceme, nějaký objekt smazat, vyplníme nejdříve zmíněné polícko a poté klikneme na SMAZAT
- poté je daný objekt smazán z databáze

UPRAVIT:

- pokud chceme, nějaký objekt upravovat, vyplníme nejdříve zmíněné polícko a poté klikneme na UPRAVIT
- poté můžeme vyplnit pouze políčka, které chceme u zvoleného objektu změnit a znovu klikneme na UPRAVIT
- poté je zvolený objekt upravený

VYPSAT:

- pred kliknutim se musíme nacházet v tabu "Vypis"
- poté vyplníme políčko příslušným názvem tabulky, ze které chceme vypsat data
- pak klikneme na VYPSAT

NACIST DATA:

- nacitat data muzete pouze jednou
- po kliknuti se nactou data z csv souboru do danych tabulek (urcenych jiz v souboru (vsechny zadane data musi byt validni))
- csv file je umístěn: "\Obchod database\Obchod database\bin\Debug\", file path v programu se nastavuje ve třídě "Form1.cs" v konstruktoru
- např: \
  Zakaznik,jmeno,email,, \
  ,Jirka,jiri@s.cz,, \
  ,Simon,simon@o.com,, \
  Objednavka,zak_id,cislo objednavky,datum,cena objednavky \
  ,13,1,2023-12-22,1000 \
  ,14,2,2022-03-09,500
