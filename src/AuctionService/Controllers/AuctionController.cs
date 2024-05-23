using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controller;

[ApiController]
[Route("api/auctions")]
public class AuctionController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly AuctionDbContext _dbContext;

    public AuctionController(AuctionDbContext dbContext, IMapper mapper)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDTO>>> Get()
    {
        var auctions = await _dbContext.Auctions
            .Include(x => x.Item)
            .OrderBy(x => x.Item.Make)
            .ToListAsync();

        return _mapper.Map<List<AuctionDTO>>(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDTO>> Get(Guid id)
    {
        var auction = await _dbContext.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);
        return _mapper.Map<AuctionDTO>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDTO>> Post(CreateAuctionDTO auctionDTO)
    {
        var auction = _mapper.Map<Auction>(auctionDTO);

        // TODO: add current user as seller
        auction.Seller = "Saber";

        _dbContext.Add(auction);

        var result = await _dbContext.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Couldn't save changes to the DB");

        return CreatedAtAction(nameof(Get),
            new { auction.Id }, _mapper.Map<AuctionDTO>(auction));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(Guid id, UpdateAuctionDTO auctionDTO)
    {
        var auction = await _dbContext.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

        if (auction is null)
            return NotFound("Auction not found");

        auction.Item.Make = auctionDTO.Make ?? auction.Item.Make;
        auction.Item.Model = auctionDTO.Model ?? auction.Item.Model;
        auction.Item.Color = auctionDTO.Color ?? auction.Item.Color;
        auction.Item.Mileage = auctionDTO.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = auctionDTO.Year ?? auction.Item.Year;

        var result = await _dbContext.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Couldn't save changes to the DB");

        return Ok();
    }

    public async Task<ActionResult> Delete(Guid id)
    {
        var auction = await _dbContext.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

        if (auction is null)
            return NotFound();

        _dbContext.Auctions.Remove(auction);

        var result = await _dbContext.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Couldn't delete the Auction");

        return Ok();
    }
}
