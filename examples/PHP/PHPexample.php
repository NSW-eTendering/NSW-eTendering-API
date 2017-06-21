<?php
// Search for closed tenders and find the contracts which resulted from them

// Search for contracts
$baseurl = 'http://tenders.nsw.gov.au/';
$tendersearch = getAndDecode($baseurl.'?event=public.api.tender.search&ResultsPerPage=99');	// &category=95 works on dev
print '<table><tr><th>Tenders</th><th>Contracts</th><th>Planned Procurements</th></tr>'.PHP_EOL;
// For each tender in the search results,  get the full view

foreach($tendersearch->releases as $release) {
    print '<tr>';
    $RFTUUID = $release->tender->RFTUUID;
    $tender = getAndDecode($baseurl.'?event=public.api.tender.view&RFTUUID='.$RFTUUID);
    print '<td><a href="'.$baseurl.'?event=public.rft.show&RFTUUID='.$RFTUUID.'">'.$release->tender->title.'</a></td>';
    // For each search result,  get the related contracts
    print '<td>';
    if (property_exists($tender->releases[0]->tender,'relatedCN')) {
	foreach($tender->releases[0]->tender->relatedCN as &$CNUUID) {
	    $contract = getAndDecode($baseurl.'?event=public.api.contract.view&CNUUID='.$CNUUID);
	    print '<a href="'.$baseurl.'?event=public.CN.view&CNUUID='.$CNUUID.'">'.$contract->releases[0]->awards[0]->title.'</a><br/>';
	}
    } else {
      print 'No contracts';
    }
    print '</td><td>';
    if (property_exists($tender->releases[0]->tender,'PlannedProcurementUUID')) {
	foreach($tender->releases[0]->tender->PlannedProcurementUUID as &$PlannedProcurementUUID) {
	    $plannedProcurement = getAndDecode($baseurl.'?event=public.api.planning.view&plannedProcurementUUID='.$plannedProcurementUUID);
	    print $plannedProcurement->releases[0]->tender->description;
// ?event=public.advancedsearch.keyword&keyword=MOE-BH-PP001	    
	}
    } else {
      print 'No planned procurement';
    }
    sleep(1);  // if your application runs too fast, it may be blocked
    print '</td></tr>'.PHP_EOL;
}

print '</table>';


function getAndDecode($url) {
	$content = file_get_contents($url);
	if ($content === false) {
	  print 'Error getting http response: '.$content;
	  exit;
	}
    $json = json_decode($content);

    if (json_last_error() === JSON_ERROR_NONE && !property_exists($json,'errors') ) {
      return $json;
    } else {
      print 'Error parsing json document from '.$url;
      print $content;
      exit;
    }
}
?>