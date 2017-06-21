<?php
// Search for closed tenders and find the contracts which resulted from them

// Search for contracts
$baseurl = 'http://tenders.nsw.gov.au/';
$contractsearch = getAndDecode($baseurl.'?event=public.api.contract.search&ResultsPerPage=999');	// &category=95 works on dev
print '<table><tr><th>Contracts</th><th>Tenders</th><th>Planned Procurements</th></tr>'.PHP_EOL;
// For each tender in the search results,  get the full view

foreach($contractsearch->releases as $release) {
    print '<tr><td>';
// print_r($release);    
    
    // up to 90 days after a contract ends, it is "lapsed" and no longer displayed on the public site.  Many of these links will fail
    if (property_exists($release->awards[0],'CNUUID')) {
	    print '<a href="'.$baseurl.'?event=public.CN.view&CNUUID='.$release->awards[0]->CNUUID.'">'.$release->awards[0]->title.'</a>';
	    $contract = getAndDecode($baseurl.'?event=public.api.contract.view&CNUUID='.$release->awards[0]->CNUUID);
    } elseif (property_exists($tender->releases[0]->awards[0],'SONUUID')) {
	foreach($tender->releases[0]->awards[0]->SONUUID as &$SONUUID) {
	    print '<a href="'.$baseurl.'?event=public.SON.view&SONUUID='.$release->awards[0]->SOCNUUID.'">'.$releases->awards[0]->title.'</a>';
	    $contract = getAndDecode($baseurl.'?event=public.api.contract.view&$SONUUID='.$release->awards[0]->SONUUID);

	    
	}
    }
    print '</td><td>';
    
        
    if (property_exists($contract->releases[0]->awards[0],'relatedRFT')) {
	foreach($contract->releases[0]->awards[0]->relatedRFT as &$relatedRFT) {
	$tender = getAndDecode($baseurl.'?event=public.api.tender.view&RFTUUID='.$relatedRFT);
	print '<a href="'.$baseurl.'?event=public.rft.show&RFTUUID='.$relatedRFT.'">'.$tender->releases[0]->tender->title.'</a>';
	
	
	}
    } else {
      print 'No tender';
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
