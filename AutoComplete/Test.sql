SELECT * FROM autocomp.raw_universe_rt_data
where Symbol ='NIFTYCOMMODITIE';
-- where Exchange=223 and Symbol='8AECQ';
SELECT * FROM autocomp.raw_performance_company_gid_data ;

-- 223 Exchange 
SELECT * FROM autocomp.raw_universe_rt_data
where Exchange=223 and Symbol='8AECQ';

SELECT count(1) FROM autocomp.raw_universe_rt_data 
where Exchange = 223;

SELECT distinct Region FROM autocomp.raw_universe_rt_data
where Exchange = 223;

-- hardcode
select count(1) from raw_index_gid_data where RTExchangeId=223;

select * from raw_index_gid_data where RTExchangeId=223 ;

-- Output
-- 4221
select * from raw_universe_index_data where rtExchangeId=223;  -- 0
-- 95769
select * from raw_universe_solr_data where rtExchangeId=223 ;  -- 1429
-- global_fund_lists.csv   286763
select * from raw_fund_data ;

